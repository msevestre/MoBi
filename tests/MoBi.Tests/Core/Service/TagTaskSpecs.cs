﻿using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using MoBi.Core.Commands;
using MoBi.Core.Domain.Model;
using MoBi.Core.Services;

using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Descriptors;

namespace MoBi.Core.Service
{
   public abstract class concern_for_TagTask : ContextSpecification<ITagTask>
   {
      private IMoBiContext _context;
      protected IBuildingBlock _buildingBlock;
      protected IObserverBuilder _observerBuilder;

      protected override void Context()
      {
         _context = A.Fake<IMoBiContext>();
         _buildingBlock = A.Fake<IBuildingBlock>();
         _observerBuilder = new ContainerObserverBuilder();
         sut = new TagTask(_context);
      }
   }

   public class When_adding_a_match_tag_to_a_taggable_object : concern_for_TagTask
   {
      private IMoBiCommand _command;

      protected override void Because()
      {
         _command = sut.AddTagCondition("toto", TagType.Match, _observerBuilder, _buildingBlock, x => x.ContainerCriteria);
      }

      [Observation]
      public void should_add_the_tag_to_the_object_and_return_the_expected_command()
      {
         _command.ShouldBeAnInstanceOf<AddMatchTagConditionCommandBase<IObserverBuilder>>();
      }
   }

   public class When_adding_a_not_match_tag_to_a_taggable_object : concern_for_TagTask
   {
      private IMoBiCommand _command;

      protected override void Because()
      {
         _command = sut.AddTagCondition("toto", TagType.NotMatch, _observerBuilder, _buildingBlock, x => x.ContainerCriteria);
      }

      [Observation]
      public void should_add_the_tag_to_the_object_and_return_the_expected_command()
      {
         _command.ShouldBeAnInstanceOf<AddNotMatchTagConditionCommandBase<IObserverBuilder>>();
      }
   }

   public class When_adding_a_match_all_tag_to_a_taggable_object : concern_for_TagTask
   {
      private IMoBiCommand _command;

      protected override void Because()
      {
         _command = sut.AddTagCondition("toto", TagType.MatchAll, _observerBuilder, _buildingBlock, x => x.ContainerCriteria);
      }

      [Observation]
      public void should_add_the_tag_to_the_object_and_return_the_expected_command()
      {
         _command.ShouldBeAnInstanceOf<AddMatchAllConditionCommandBase<IObserverBuilder>>();
      }
   }

   public class When_removing_a_not_match_tag_criteria_from_a_taggable_object : concern_for_TagTask
   {
      private IMoBiCommand _command;

      protected override void Context()
      {
         base.Context();
         _observerBuilder.ContainerCriteria = Create.Criteria(x => x.Not("TOTO"));
      }

      protected override void Because()
      {
         _command = sut.RemoveTagCondition("TOTO", TagType.NotMatch, _observerBuilder, _buildingBlock, x => x.ContainerCriteria);
      }

      [Observation]
      public void should_remove_the_tag_from_the_object_and_return_the_expected_command()
      {
         _command.ShouldBeAnInstanceOf<RemoveNotMatchTagConditionCommandBase<IObserverBuilder>>();
      }
   }


   public class When_removing_a_match_tag_criteria_from_a_taggable_object : concern_for_TagTask
   {
      private IMoBiCommand _command;

      protected override void Context()
      {
         base.Context();
         _observerBuilder.ContainerCriteria = Create.Criteria(x => x.With("TOTO"));
      }

      protected override void Because()
      {
         _command = sut.RemoveTagCondition("TOTO", TagType.Match, _observerBuilder, _buildingBlock, x => x.ContainerCriteria);
      }

      [Observation]
      public void should_remove_the_tag_from_the_object_and_return_the_expected_command()
      {
         _command.ShouldBeAnInstanceOf<RemoveMatchTagConditionCommandBase<IObserverBuilder>>();
      }
   }

   public class When_editing_a_tag_criteria_from_a_taggable_object : concern_for_TagTask
   {
      private IMoBiCommand _command;

      protected override void Context()
      {
         base.Context();
         _observerBuilder.ContainerCriteria = Create.Criteria(x => x.With("TOTO"));
      }

      protected override void Because()
      {
         _command = sut.EditTag("TATA","TOTO",  _observerBuilder, _buildingBlock, x => x.ContainerCriteria);
      }

      [Observation]
      public void should_remove_the_tag_from_the_object_and_return_the_expected_command()
      {
         _command.ShouldBeAnInstanceOf<EditTagCommand<IObserverBuilder>>();
      }
   }
}