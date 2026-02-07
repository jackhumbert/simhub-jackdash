using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace User.CornerSpeed
{
  internal class iRacingGroupPresenter : Control
  {
    public static readonly DependencyProperty ItemSourceProperty = DependencyProperty.Register(nameof (ItemSource), typeof (IEnumerable), typeof (iRacingGroupPresenter), new PropertyMetadata((PropertyChangedCallback) null));
    //public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(nameof (Scale), typeof (double), typeof (iRacingGroupPresenter), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty UseCacheProperty = DependencyProperty.Register(nameof (UseCache), typeof (bool), typeof (iRacingGroupPresenter), new PropertyMetadata((object) false));

    static iRacingGroupPresenter()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (iRacingGroupPresenter), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (iRacingGroupPresenter)));
    }
        
    public IEnumerable ItemSource
    {
      get => (IEnumerable) this.GetValue(iRacingGroupPresenter.ItemSourceProperty);
      set => this.SetValue(iRacingGroupPresenter.ItemSourceProperty, (object) value);
    }
    //public double Scale
    //{
    //  get => (double) this.GetValue(iRacingGroupPresenter.ScaleProperty);
    //  set => this.SetValue(iRacingGroupPresenter.ScaleProperty, (object) value);
    //}

    public bool UseCache
    {
      get => (bool) this.GetValue(iRacingGroupPresenter.UseCacheProperty);
      set => this.SetValue(iRacingGroupPresenter.UseCacheProperty, (object) value);
    }
  }

  internal class iRacingSubGroupPresenter : Control
  {
    public static readonly DependencyProperty ItemSourceProperty = DependencyProperty.Register(nameof (ItemSource), typeof (IEnumerable), typeof (iRacingSubGroupPresenter), new PropertyMetadata((PropertyChangedCallback) null));
    //public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(nameof (Scale), typeof (double), typeof (iRacingSubGroupPresenter), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty UseCacheProperty = DependencyProperty.Register(nameof (UseCache), typeof (bool), typeof (iRacingSubGroupPresenter), new PropertyMetadata((object) false));

    static iRacingSubGroupPresenter()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (iRacingSubGroupPresenter), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (iRacingSubGroupPresenter)));
    }
        
    public IEnumerable ItemSource
    {
      get => (IEnumerable) this.GetValue(iRacingSubGroupPresenter.ItemSourceProperty);
      set => this.SetValue(iRacingSubGroupPresenter.ItemSourceProperty, (object) value);
    }
    //public double Scale
    //{
    //  get => (double) this.GetValue(iRacingSubGroupPresenter.ScaleProperty);
    //  set => this.SetValue(iRacingSubGroupPresenter.ScaleProperty, (object) value);
    //}

    public bool UseCache
    {
      get => (bool) this.GetValue(iRacingSubGroupPresenter.UseCacheProperty);
      set => this.SetValue(iRacingSubGroupPresenter.UseCacheProperty, (object) value);
    }
  }
}
