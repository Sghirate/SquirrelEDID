﻿using SquirrelEDID.Utilities.Messaging;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace SquirrelEDID.View.Controls
{
    public enum SlideDirection
    {
        Left,
        Right,
        Up,
        Down
    }

    [TemplatePart(Type = typeof(ContentControl), Name = PART_FRONT)]
    [TemplatePart(Type = typeof(ContentControl), Name = PART_BACK)]
    public class SlideControl : Control
    {
        #region Constants
        private const string PART_FRONT = "PART_Front";
        private const string PART_BACK = "PART_Back";
        #endregion

        #region Fields
        private ContentControl _front;
        private ContentControl _back;
        private TranslateTransform _frontTransform;
        private TranslateTransform _backTransform;
        private bool _inAnimation = false;
        private Tuple<FrameworkElement, SlideDirection> _cache;
        #endregion

        #region DependencyProperties
        public static readonly DependencyProperty ViewProperty = DependencyProperty.Register("View", typeof(FrameworkElement), typeof(SlideControl), new PropertyMetadata(new PropertyChangedCallback(ViewChanged)));
        public static readonly DependencyProperty TransitionTimeProperty = DependencyProperty.Register("TransitionTime", typeof(Duration), typeof(SlideControl)); 
        #endregion

        #region Properties
        public FrameworkElement View
        {
            get { return (FrameworkElement)GetValue(ViewProperty); }
            set { SetValue(ViewProperty, value); }
        }
        public Duration TransitionTime
        {
            get { return (Duration)GetValue(TransitionTimeProperty); }
            set { SetValue(TransitionTimeProperty, value); }
        } 
        #endregion

        #region Constructors
        public SlideControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SlideControl), new FrameworkPropertyMetadata(typeof(SlideControl)));
            Messenger<FrameworkElement, SlideDirection>.AddListener(OnSlide);
        } 
        #endregion

        #region Methods
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _back = Template.FindName(PART_BACK, this) as ContentControl;
            _front = Template.FindName(PART_FRONT, this) as ContentControl;
            _frontTransform = new TranslateTransform();
            _backTransform = new TranslateTransform();
            _front.RenderTransform = _frontTransform;
            _back.RenderTransform = _backTransform;

            _back.Background = Brushes.Green;

            SetView(View);
        }

        private static void ViewChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement ele = e.NewValue as FrameworkElement;
            SlideControl c = obj as SlideControl;

            if (c != null)
                c.SetView(ele);
        }

        private void SetView(FrameworkElement ele)
        {
            if (_front == null || _back == null)
                return;

            _inAnimation = false;
            _front.Content = null;
            _back.Content = ele;
            // inb4 nasty hack. Required to reset transformation of the background content control
            _backTransform = new TranslateTransform();
            _back.RenderTransform = _backTransform;
            _front.Visibility = System.Windows.Visibility.Hidden;

            // Check, if we have a queued animation, and if yes: play
            if (_cache != null)
            {
                OnSlide(_cache.Item1, _cache.Item2);
                _cache = null;
            }
        }

        private void OnSlide(FrameworkElement content, SlideDirection direction)
        {
            if (_back.Content == content)
                return;

            if (_inAnimation)
            {
                _cache = new Tuple<FrameworkElement, SlideDirection>(content, direction);
                return;
            }

            _inAnimation = true;
            DoubleAnimation daFront = new DoubleAnimation{ Duration = TransitionTime };
            DoubleAnimation daBack = new DoubleAnimation { Duration = TransitionTime };
            daBack.Completed += (s, e) => { SetView(content); };
            DependencyProperty dp = TranslateTransform.XProperty;
            switch (direction)
            {
                case SlideDirection.Left:
                    dp = TranslateTransform.XProperty;
                    daFront.From = _front.ActualWidth;
                    daFront.To = 0;
                    daBack.From = 0;
                    daBack.To = -_back.ActualWidth;
                    break;
                case SlideDirection.Right:
                    dp = TranslateTransform.XProperty;
                    daFront.From = -_front.ActualWidth;
                    daFront.To = 0;
                    daBack.From = 0;
                    daBack.To = _back.ActualWidth;
                    break;
                case SlideDirection.Up:
                    dp = TranslateTransform.YProperty;
                    daFront.From = _front.ActualHeight;
                    daFront.To = 0;
                    daBack.From = 0;
                    daBack.To = -_back.ActualHeight;
                    break;
                case SlideDirection.Down:
                    dp = TranslateTransform.YProperty;
                    daFront.From = -_front.ActualHeight;
                    daFront.To = 0;
                    daBack.From = 0;
                    daBack.To = _back.ActualHeight;
                    break;
            }

            _front.Content = content;
            _front.Visibility = System.Windows.Visibility.Visible;
            _frontTransform.BeginAnimation(dp, daFront);
            _backTransform.BeginAnimation(dp, daBack);
        } 
        #endregion
    }
}