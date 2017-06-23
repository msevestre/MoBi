﻿using System;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.FileLocker;
using MoBi.Core.Commands;
using MoBi.Core.Domain.Services;
using MoBi.Core.Domain.UnitSystem;
using MoBi.Core.Serialization.Xml.Services;
using MoBi.Core.Services;
using OSPSuite.Core;
using OSPSuite.Core.Commands;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Extensions;
using OSPSuite.Infrastructure;
using OSPSuite.Infrastructure.Journal;
using OSPSuite.Infrastructure.Serialization.ORM.History;
using IContainer = OSPSuite.Utility.Container.IContainer;

namespace MoBi.Core.Domain.Model
{
   public interface IMoBiContext : IOSPSuiteExecutionContext<IMoBiProject>, IWorkspace
   {
      IMoBiDimensionFactory DimensionFactory { get; }
      IObjectBaseFactory ObjectBaseFactory { get; }
      IWithIdRepository ObjectRepository { get; }
      T Create<T>(string id) where T : IObjectBase;
      T Create<T>() where T : IObjectBase;

      IMoBiHistoryManager HistoryManager { get; set; }
      IObjectPathFactory ObjectPathFactory { get; }
      void NewProject();
      void LoadFrom(IMoBiProject project);

      /// <summary>
      ///    Converts the given <paramref name="value" /> to a representation that should be saved in commands. The returned
      ///    value is typically used when restoring the execution data to execute the undo command. To restore the value, use
      ///    <see cref="DeserializeValueTo" />
      /// </summary>
      string SerializeValue(object value);

      /// <summary>
      ///    Converts the given <paramref name="valueAsString" /> to the corresponding object of type
      ///    <paramref name="propertyType" />. The <paramref name="valueAsString" />
      ///    is typically created using the <see cref="SerializeValue" />
      /// </summary>
      object DeserializeValueTo(Type propertyType, string valueAsString);

      void UnregisterSimulation(IMoBiSimulation simulation);
   }

   public class MoBiContext : Workspace<IMoBiProject>, IMoBiContext
   {
      private readonly IXmlSerializationService _serializationService;
      private readonly IHistoryManagerFactory _historyManagerFactory;
      private readonly IRegisterAllVisitor _registerAllVisitor;
      private readonly IUnregisterVisitor _unregisterVisitor;
      private readonly IClipboardManager _clipboardManager;
      private readonly IContainer _container;
      private readonly IObjectTypeResolver _objectTypeResolver;
      private readonly ICloneManagerForBuildingBlock _cloneManager;
      private readonly ILazyLoadTask _lazyLoadTask;

      public IMoBiHistoryManager HistoryManager { get; set; }
      public IMoBiDimensionFactory DimensionFactory { get; private set; }
      public IObjectBaseFactory ObjectBaseFactory { get; private set; }
      public IObjectPathFactory ObjectPathFactory { get; private set; }
      public ICoreCalculationMethodRepository CalculatonMethodRepository { get; set; }
      public IEventPublisher EventPublisher { get; private set; }
      public IWithIdRepository ObjectRepository { get; private set; }

      public MoBiContext(IObjectBaseFactory objectBaseFactory, IMoBiDimensionFactory dimensionFactory, IEventPublisher eventPublisher,
         IXmlSerializationService serializationService, IObjectPathFactory objectPathFactory, IWithIdRepository objectBaseRepository,
         IHistoryManagerFactory historyManagerFactory, IRegisterAllVisitor registerAllVisitor, IUnregisterVisitor unregisterVisitor,
         IClipboardManager clipboardManager, IContainer container, IObjectTypeResolver objectTypeResolver,
         ICloneManagerForBuildingBlock cloneManager, IJournalSession journalSession, IFileLocker fileLocker, ILazyLoadTask lazyLoadTask) : base(eventPublisher, journalSession, fileLocker)
      {
         ObjectBaseFactory = objectBaseFactory;
         ObjectRepository = objectBaseRepository;
         EventPublisher = eventPublisher;
         DimensionFactory = dimensionFactory;
         ObjectPathFactory = objectPathFactory;
         _serializationService = serializationService;
         _container = container;
         _objectTypeResolver = objectTypeResolver;
         _cloneManager = cloneManager;
         _lazyLoadTask = lazyLoadTask;
         _historyManagerFactory = historyManagerFactory;
         _registerAllVisitor = registerAllVisitor;
         _unregisterVisitor = unregisterVisitor;
         _clipboardManager = clipboardManager;
      }

      public IMoBiProject CurrentProject
      {
         get { return _project; }
         set { _project = value; }
      }

      public T Create<T>() where T : IObjectBase
      {
         return ObjectBaseFactory.Create<T>();
      }

      public T Get<T>(string id) where T : class, IWithId
      {
         return Get(id) as T;
      }

      public IWithId Get(string id)
      {
         return ObjectRepository.Get(id);
      }

      public T Create<T>(string id) where T : IObjectBase
      {
         return id != null ? ObjectBaseFactory.Create<T>(id) : ObjectBaseFactory.Create<T>();
      }

      public void AddToHistory(ICommand command)
      {
         HistoryManager.AddCommand(command);
      }

      public void ProjectChanged()
      {
         if (CurrentProject == null) return;
         CurrentProject.HasChanged = true;
      }

      public T Clone<T>(T objectToClone) where T : class, IObjectBase
      {
         return _cloneManager.Clone(objectToClone);
      }

      public void Load(IObjectBase objectBase)
      {
         _lazyLoadTask.Load(objectBase as ILazyLoadable);
      }

      public IProject Project => CurrentProject;


      public void NewProject()
      {
         Clear();
         CurrentProject = ObjectBaseFactory.Create<IMoBiProject>();

         HistoryManager = _historyManagerFactory.Create() as IMoBiHistoryManager;
         LoadFrom(CurrentProject);
         AddToHistory(new CreateProjectCommand().Run(this));
      }

      public override void Clear()
      {
         DimensionFactory.ProjectFactory = null;
         HistoryManager = null;
         ObjectRepository.Clear();
         _clipboardManager.Clear();
         base.Clear();
      }

      public byte[] Serialize<T>(T toSerialize)
      {
         return _serializationService.SerializeAsBytes(toSerialize);
      }

      public T Deserialize<T>(byte[] serializationStream)
      {
         return _serializationService.Deserialize<T>(serializationStream, CurrentProject);
      }

      public void PublishEvent<T>(T eventToPublish)
      {
         EventPublisher.PublishEvent(eventToPublish);
      }

      public void Register(IWithId objectBase)
      {
         _registerAllVisitor.RegisterAllIn(objectBase);
      }

      public void Unregister(IWithId objectBase)
      {
         _unregisterVisitor.UnregisterAllIn(objectBase);
      }

      public T Resolve<T>()
      {
         return _container.Resolve<T>();
      }

      public void LoadFrom(IMoBiProject project)
      {
         DimensionFactory.ProjectFactory = project.DimensionFactory;
         CurrentProject = project;
         _registerAllVisitor.Register(project);
      }

      public string SerializeValue(object value)
      {
         if (value == null)
            return String.Empty;

         if (value.IsAnImplementationOf<IObjectBase>())
            return value.DowncastTo<IObjectBase>().Id;

         if (value.IsAnImplementationOf<IDimension>())
            return value.DowncastTo<IDimension>().Name;

         if (value.IsAnImplementationOf<IObjectPath>())
            return value.DowncastTo<IObjectPath>().PathAsString;

         if (value.IsAnImplementationOf<Unit>())
            return value.DowncastTo<Unit>().Name;

         return value.ConvertedTo<String>();
      }

      public object DeserializeValueTo(Type propertyType, string valueAsString)
      {
         if (valueAsString.IsNullOrEmpty())
         {
            if (propertyType.IsAnImplementationOf<string>())
               return string.Empty;

            if (!propertyType.IsValueType)
               return null;
         }

         if (propertyType.IsAnImplementationOf<IObjectBase>())
            return ObjectRepository.Get(valueAsString);

         if (propertyType.IsAnImplementationOf<IDimension>())
            return DimensionFactory.Dimension(valueAsString);

         if (propertyType.IsAnImplementationOf<IObjectPath>())
            return ObjectPathFactory.CreateObjectPathFrom(valueAsString.ToPathArray());

         if (propertyType.IsAnImplementationOf<Unit>())
            return DimensionFactory.DimensionForUnit(valueAsString).UnitOrDefault(valueAsString);

         if (propertyType.IsAnImplementationOf<Enum>())
            return Enum.Parse(propertyType, valueAsString);

         return Convert.ChangeType(valueAsString, propertyType);
      }

      public void UnregisterSimulation(IMoBiSimulation simulation)
      {
         Unregister(simulation);
         unregisterCachedFormulaInModel(simulation.Model);
      }

      private void unregisterCachedFormulaInModel(IModel model)
      {
         //Cacheable Formulas are not automatically unregistered we need to do this in an extra step
         unregisterAllCachableFormulasIn(model.Root);
         unregisterAllCachableFormulasIn(model.Neighborhoods);
      }

      private void unregisterAllCachableFormulasIn(OSPSuite.Core.Domain.IContainer container)
      {
         container.GetAllChildren<IUsingFormula>(x => x.Formula.IsCachable()).Each(x => Unregister(x.Formula));
      }

      public string TypeFor<T>(T obj) where T : class
      {
         return _objectTypeResolver.TypeFor(obj);
      }
   }
}