using System.Collections.Generic;
using MoBi.Assets;
using OSPSuite.Presentation.MenuAndBars;
using MoBi.Presentation.DTO;
using MoBi.Presentation.Presenter;
using OSPSuite.Presentation.Core;
using OSPSuite.Assets;

namespace MoBi.Presentation.MenusAndBars.ContextMenus
{
   public class ContextMenuForDescriptorCondition : ContextMenuBase
   {
      protected readonly IDescriptorConditionListPresenter _presenter;
      private readonly IViewItem _viewItem;
      private readonly bool _allowAddAllConditon;

      public ContextMenuForDescriptorCondition(IDescriptorConditionListPresenter presenter, IViewItem viewItem, bool allowAddAllConditon = false)
      {
         _presenter = presenter;
         _viewItem = viewItem;
         _allowAddAllConditon = allowAddAllConditon;
      }

      protected virtual IMenuBarItem CreateRemoveCommand(IDescriptorConditionDTO dto)
      {
         return CreateMenuButton.WithCaption(AppConstants.Captions.RemoveCondition)
            .WithIcon(ApplicationIcons.Delete)
            .WithActionCommand(() => _presenter.RemoveCondition(dto));
      }

      protected virtual IMenuBarItem CreateAddNewMatchCondition()
      {
         return CreateMenuButton.WithCaption(AppConstants.Captions.NewMatchTagCondition)
            .WithActionCommand(() => _presenter.NewMatchTagCondition());
      }

      protected virtual IMenuBarItem CreateAddAllCondition()
      {
         return CreateMenuButton.WithCaption(AppConstants.Captions.AddMatchAllCondition)
            .WithActionCommand(() => _presenter.NewMatchAllCondition());
      }

      protected virtual IMenuBarItem CreateAddNewNotMatchTagCondition()
      {
         return CreateMenuButton.WithCaption(AppConstants.Captions.NewNotMatchTagCondition)
            .WithActionCommand(() => _presenter.NewNotMatchTagCondition());
      }

      public override IEnumerable<IMenuBarItem> AllMenuItems()
      {
         var allItems = new List<IMenuBarItem>
         {
            CreateAddNewMatchCondition(),
            CreateAddNewNotMatchTagCondition()
         };

         if (_allowAddAllConditon)
            allItems.Add(CreateAddAllCondition());

         var dto = _viewItem as IDescriptorConditionDTO;
         if (dto != null)
            allItems.Add(CreateRemoveCommand(dto).AsGroupStarter());

         return allItems;
      }
   }
}