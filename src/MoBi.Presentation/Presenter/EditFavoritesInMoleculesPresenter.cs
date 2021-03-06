﻿using System;
using System.Collections.Generic;
using MoBi.Assets;
using MoBi.Core.Services;
using MoBi.Presentation.Mappers;
using MoBi.Presentation.Tasks.Interaction;
using MoBi.Presentation.Views;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Repositories;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;

namespace MoBi.Presentation.Presenter
{
   public interface IEditFavoritesInMoleculesPresenter : IEditFavoritesPresenter<IBuildingBlock<IMoleculeBuilder>>
   {
   }

   internal class EditFavoritesInMoleculesPresenter : EditFavoritesInBuildindBlockPresenter<IMoleculeBuilder>,IEditFavoritesInMoleculesPresenter
   {
      public EditFavoritesInMoleculesPresenter(IEditFavoritesView view, IQuantityTask quantityTask, IInteractionTaskContext interactionTaskContext, IFormulaToFormulaBuilderDTOMapper formulaMapper, IParameterToFavoriteParameterDTOMapper favoriteMapper, IFavoriteRepository favoriteRepository, IInteractionTasksForParameter parameterTask, IFavoriteTask favoriteTask, IEntityPathResolver entityPathResolver, IViewItemContextMenuFactory contextMenuFactory)
         : base(
            view, quantityTask, interactionTaskContext, formulaMapper, favoriteMapper, favoriteRepository, parameterTask,
            favoriteTask, entityPathResolver, contextMenuFactory)
      {
         var captions = new Dictionary<PathElement, string>();
         captions.Add(PathElement.TopContainer, AppConstants.Captions.Molecule);
         captions.Add(PathElement.Container, $"{ObjectTypes.TransporterMoleculeContainer}/{ObjectTypes.InteractionContainer}");
         captions.Add(PathElement.BottomCompartment, ObjectTypes.ActiveTransport);
         _view.SetCaptions(captions);
      }

      protected override void UpdateSpecialColumnsVisibility()
      {
         base.UpdateSpecialColumnsVisibility();
         _view.SetVisibility(PathElement.Molecule, isVisible: false);
      }
   }
}