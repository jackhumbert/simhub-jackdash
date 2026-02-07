

// SimHub.Plugins, Version=1.0.9524.22766, Culture=neutral, PublicKeyToken=null
// SimHub.Plugins.OutputPlugins.GraphicalDash.LayerPresenter
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace User.CornerSpeed {

public class iRacingLayerPresenter : Control
{
        
    public static readonly DependencyProperty ItemSourceProperty = DependencyProperty.Register(nameof(ItemSource), typeof(IEnumerable), typeof(iRacingLayerPresenter), new PropertyMetadata((PropertyChangedCallback)null));
        public static readonly DependencyProperty RepeatItemSourceProperty = DependencyProperty.Register(nameof(RepeatItemSource), typeof(IEnumerable), typeof(iRacingLayerPresenter), new PropertyMetadata((PropertyChangedCallback)null));
        public static readonly DependencyProperty UseCacheProperty = DependencyProperty.Register(nameof(UseCache), typeof(bool), typeof(iRacingLayerPresenter), new PropertyMetadata((object)false));

        static iRacingLayerPresenter()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(iRacingLayerPresenter), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(iRacingLayerPresenter)));
        }

        public IEnumerable ItemSource
        {
            get => (IEnumerable)this.GetValue(iRacingLayerPresenter.ItemSourceProperty);
            set => this.SetValue(iRacingLayerPresenter.ItemSourceProperty, (object)value);
        }

        public IEnumerable RepeatItemSource
        {
            get => (IEnumerable)this.GetValue(iRacingLayerPresenter.RepeatItemSourceProperty);
            set => this.SetValue(iRacingLayerPresenter.RepeatItemSourceProperty, (object)value);
        }

        public bool UseCache
        {
            get => (bool)this.GetValue(iRacingLayerPresenter.UseCacheProperty);
            set => this.SetValue(iRacingLayerPresenter.UseCacheProperty, (object)value);
        }

        //public static readonly DependencyProperty ItemSourceProperty;

        //       public static readonly DependencyProperty RepeatItemSourceProperty;

        //       public static readonly DependencyProperty UseCacheProperty;

        //       public IEnumerable ItemSource
        //       {
        //           get
        //           {
        //               return (IEnumerable)GetValue(ItemSourceProperty);
        //           }
        //           set
        //           {
        //               SetValue(ItemSourceProperty, value);
        //           }
        //       }

        //       public IEnumerable RepeatItemSource
        //       {
        //           get
        //           {
        //               return (IEnumerable)GetValue(RepeatItemSourceProperty);
        //           }
        //           set
        //           {
        //               SetValue(RepeatItemSourceProperty, value);
        //           }
        //       }

        //       public bool UseCache
        //       {
        //           get
        //           {
        //               return (bool)GetValue(UseCacheProperty);
        //           }
        //           set
        //           {
        //               SetValue(UseCacheProperty, value);
        //           }
        //       }

        //       static iRacingLayerPresenter()
        //       {
        //           ItemSourceProperty = DependencyProperty.Register("ItemSource", typeof(IEnumerable), typeof(iRacingLayerPresenter), new PropertyMetadata(null));
        //           RepeatItemSourceProperty = DependencyProperty.Register("RepeatItemSource", typeof(IEnumerable), typeof(iRacingLayerPresenter), new PropertyMetadata(null));
        //           UseCacheProperty = DependencyProperty.Register("UseCache", typeof(bool), typeof(iRacingLayerPresenter), new PropertyMetadata(false));
        //           FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(iRacingLayerPresenter), new FrameworkPropertyMetadata(typeof(iRacingLayerPresenter)));
        //       }
    }

}