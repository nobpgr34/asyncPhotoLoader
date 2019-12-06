using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace WpfPhotoViewer
{
    public interface IDragDropHandler
    {
        bool CanDrop(IDataObject dropObject, IEnumerable dropTarget);

        void OnDrop(IDataObject dropObject, IEnumerable dropTarget);
    }

    public static class DragDropBehaviour
    {
        #region DragDropHandler

        #region DragDropHandler DependencyProperty

        /// 

        /// attached property that handles drag and drop
        /// 

        public static readonly DependencyProperty DragDropHandlerProperty =
           DependencyProperty.RegisterAttached("DragDropHandler",
           typeof(IDragDropHandler),
           typeof(DragDropBehaviour),
           new PropertyMetadata(null, new ExecuteDragDropBehaviour().PropertyChangedHandler));

        public static void SetDragDropHandler(DependencyObject o, object propertyValue)
        {
            o.SetValue(DragDropHandlerProperty, propertyValue);
        }
        public static object GetDragDropHandler(DependencyObject o)
        {
            return o.GetValue(DragDropHandlerProperty);
        }

        #endregion

        internal abstract class DragDropBehaviourBase
        {
            protected DependencyProperty _property;

            /// <summary>
            /// attach the events
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="oldValue"></param>
            /// <param name="newValue"></param>
            protected abstract void AdjustEventHandlers(DependencyObject sender,
                                                        object oldValue, object newValue);

            /// <summary>
            /// Listens for a change in the DependencyProperty
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public void PropertyChangedHandler(DependencyObject sender,
                                               DependencyPropertyChangedEventArgs e)
            {
                if (_property == null)
                {
                    _property = e.Property;
                }

                object oldValue = e.OldValue;
                object newValue = e.NewValue;

                AdjustEventHandlers(sender, oldValue, newValue);
            }
        }

        /// <summary>
        /// an internal class to handle listening for the drop event and executing the dropaction
        /// </summary>
        private class ExecuteDragDropBehaviour : DragDropBehaviourBase
        {
            /// <summary>
            /// attach the events
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="oldValue"></param>
            /// <param name="newValue"></param>
            protected override void AdjustEventHandlers(DependencyObject sender,
                                                   object oldValue, object newValue)
            {
                var element = sender as UIElement;
                if (element == null) { return; }

                if (oldValue != null)
                {
                    element.RemoveHandler(UIElement.DropEvent,
                                          new DragEventHandler(ReceiveDrop));
                }

                if (newValue != null)
                {
                    element.AddHandler(UIElement.DropEvent,
                                       new DragEventHandler(ReceiveDrop));
                }
            }

            /// <summary>
            /// eventhandler that gets executed when the DropEvent fires
            /// </summary>
            private void ReceiveDrop(object sender, DragEventArgs e)
            {
                var dp = sender as DependencyObject;
                if (dp == null)
                    return;

                var action = dp.GetValue(_property) as IDragDropHandler;
                if (action == null)
                    return;

                IEnumerable dropTarget = null;
                if (sender is ItemsControl)
                    dropTarget = (sender as ItemsControl).ItemsSource;

                if (action.CanDrop(e.Data, dropTarget))
                    action.OnDrop(e.Data, dropTarget);
                else
                    e.Handled = true;
            }
        }

        #endregion
  
        #region DragDropHandler
        #endregion

        #region IsDragSource DependencyProperty

        /// 

        /// attached property that defines if the source is a drag source
        /// 

        public static readonly DependencyProperty IsDragSourceProperty =
           DependencyProperty.RegisterAttached("IsDragSource", 
           typeof(bool?), 
           typeof(DragDropBehaviour),
           new PropertyMetadata(null, new IsDragSourceBehaviour().PropertyChangedHandler));

        public static void SetIsDragSource(DependencyObject o, object propertyValue)
        {
            o.SetValue(IsDragSourceProperty, propertyValue);
        }
        public static object GetIsDragSource(DependencyObject o)
        {
            return o.GetValue(IsDragSourceProperty);
        }

        #endregion

        /// <summary>
        /// Internal class that starts the draging
        /// </summary>
        private class IsDragSourceBehaviour : DragDropBehaviourBase
        {
            /// <summary>
            /// Hattach the events
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="oldValue"></param>
            /// <param name="newValue"></param>
            protected override void AdjustEventHandlers(DependencyObject sender, 
                                                        object oldValue, object newValue)
            {
                var element = sender as UIElement;
                if (element == null) 
                    return;

                if (oldValue != null)
                {
                    element.RemoveHandler(UIElement.MouseMoveEvent, 
                                          new MouseEventHandler(OnMouseMove));
                }

                if (newValue != null && newValue is bool && (bool)newValue)
                {
                    element.AddHandler(UIElement.MouseMoveEvent, 
                                       new MouseEventHandler(OnMouseMove));
                }
            }

            /// <summary>
            /// eventhandler for the MouseMoveEvent
            /// </summary>
            private void OnMouseMove(object sender, MouseEventArgs e)
            {
                if (sender is Selector && e.LeftButton == MouseButtonState.Pressed)
                {
                    var lst = (Selector)sender;
                    var selectedItem = lst.SelectedItem;

                    var dragDropEffect = DragDropEffects.Move;

                    if (dragDropEffect != DragDropEffects.None)
                    {
                        DragDropEffects enmEffect = 
                                   DragDrop.DoDragDrop(sender as DependencyObject, 
                                                       selectedItem, dragDropEffect);
                    }
                }
            }
        }
   }
}
