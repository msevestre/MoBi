using OSPSuite.Assets;
using DevExpress.XtraEditors;
using MoBi.Assets;
using MoBi.Presentation.Presenter;
using MoBi.Presentation.Views;
using MoBi.UI.Extensions;
using OSPSuite.Presentation;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;

namespace MoBi.UI.Views
{
   public partial class EditReactionBuildingBlockView : EditBuildingBlockBaseView, IEditReactionBuildingBlockView
   {
      public EditReactionBuildingBlockView(IMainView mainView)
         : base(mainView)
      {
         InitializeComponent();
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         tabFavorites.InitWith(Captions.Favorites, ApplicationIcons.Favorites);
         splitContainerControl1.CollapsePanel = SplitCollapsePanel.Panel1;
         EditCaption = AppConstants.Captions.Reactions;
      }

      protected override int TopicId => HelpId.MoBi_ModelBuilding_Reactions;

      public void AttachPresenter(IEditReactionBuildingBlockPresenter presenter)
      {
         _presenter = presenter;
      }

      public void SetEditReactionView(IView view)
      {
         splitContainerControl1.Panel2.FillWith(view);
      }

      public void SetReactionListView(IView view)
      {
         tabList.FillWith(view);
      }

      public void SetReactionDiagram(IView view)
      {
         tabFlowChart.FillWith(view);
      }

      public void SetFavoritesReactionView(IView view)
      {
         tabFavorites.FillWith(view);
      }

      public override ApplicationIcon ApplicationIcon
      {
         get { return ApplicationIcons.Reaction; }
      }
   }
}