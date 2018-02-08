﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MoBi.Core.Events;
using MoBi.Core.Services;
using MoBi.Presentation.DTO;
using MoBi.Presentation.Mappers;
using MoBi.Presentation.Tasks.Interaction;
using MoBi.Presentation.Views;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Utility.Extensions;

namespace MoBi.Presentation.Presenter
{
   public interface IUserDefinedParametersPresenter : IEditParameterListPresenter
   {
      void ShowUserDefinedParametersIn(IContainer container);
      void ShowUserDefinedParametersIn(IEnumerable<IContainer> containers);
      Action ColumnConfiguration { get; set; }
   }

   public class UserDefinedParametersPresenter : AbstractParameterBasePresenter<IEditParameterListView, IEditParameterListPresenter>, IUserDefinedParametersPresenter
   {
      private readonly IParameterToParameterDTOMapper _parameterDTOMapper;
      private readonly IViewItemContextMenuFactory _viewItemContextMenuFactory;
      private IEnumerable<IContainer> _containers;
      public Action ColumnConfiguration { get; set; } = () => { };

      public UserDefinedParametersPresenter(IEditParameterListView view,
         IQuantityTask quantityTask,
         IInteractionTaskContext interactionTaskContext,
         IFormulaToFormulaBuilderDTOMapper formulaMapper,
         IInteractionTasksForParameter parameterTask,
         IFavoriteTask favoriteTask,
         IParameterToParameterDTOMapper parameterDTOMapper,
         IViewItemContextMenuFactory viewItemContextMenuFactory) : base(view, quantityTask, interactionTaskContext, formulaMapper, parameterTask, favoriteTask)
      {
         _parameterDTOMapper = parameterDTOMapper;
         _viewItemContextMenuFactory = viewItemContextMenuFactory;
      }

      protected override void RefreshViewAndSelect(IParameterDTO parameterDTO)
      {
         refreshView();
      }

      public void ShowUserDefinedParametersIn(IContainer container)
      {
         ShowUserDefinedParametersIn(new[] {container});
      }

      public void ShowUserDefinedParametersIn(IEnumerable<IContainer> containers)
      {
         _containers = containers;
         refreshView();
      }

      private void refreshView()
      {
         var parameters = _containers
            .SelectMany(c => c.GetAllChildren<IParameter>(x => !x.IsDefault))
            .MapAllUsing(_parameterDTOMapper)
            .Cast<ParameterDTO>().ToList();

         _view.BindTo(parameters);

         ColumnConfiguration();
      }

      public void ShowContextMenu(IViewItem viewItem, Point popupLocation)
      {
         var contextMenu = _viewItemContextMenuFactory.CreateFor(viewItem, this);
         contextMenu.Show(_view, popupLocation);
      }

      public void GoTo(ParameterDTO parameterDTO)
      {
         if (parameterDTO == null)
            return;

         var parameter = GetParameterFrom(parameterDTO);
         _interactionTaskContext.Context.PublishEvent(new EntitySelectedEvent(parameter, this));
      }
   }
}