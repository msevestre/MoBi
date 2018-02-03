﻿using System.Collections.Generic;
using MoBi.Presentation.DTO;
using MoBi.Presentation.Presenter;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Views;

namespace MoBi.Presentation.Views
{
   public interface IEditParameterListView : IView<IEditParameterListPresenter>
   {
      void BindTo(IEnumerable<ParameterDTO> parameters);
      void Select(ParameterDTO parameterDTO);
      void SetCaptions(IDictionary<PathElement, string> captions);
      void SetVisibility(PathElement pathElement, bool isVisible);
   }
}