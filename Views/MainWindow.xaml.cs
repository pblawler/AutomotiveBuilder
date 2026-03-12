using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using AutomotiveBuilder.DAL;
using AutomotiveBuilder.Views;
using DragEventArgs = System.Windows.DragEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace AutomotiveBuilder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer initTmr;

        // Drag-drop fields
        private System.Windows.Point _startPoint;
        private Component? _draggedItem;
        private bool _isDragging;
        private DragAdorner? _dragAdorner;
        private AdornerLayer? _adornerLayer;
        private TreeViewItem? _lastHighlightedItem;

        public MainWindow()
        {
            InitializeComponent();

            initTmr = new DispatcherTimer();
            initTmr.Interval = TimeSpan.FromMilliseconds(2000);
            initTmr.Tick += OnTimedEvent;
            initTmr.Start();
        }

        private void OnTimedEvent(object sender, EventArgs e)
        {
            MainViewModel VM = (MainViewModel)this.DataContext;
            if(!VM.Loaded) return;
            bool expanded = false;

            foreach (var item in trvBOM.Items)
            {
                // Get the TreeViewItem container for the data item
                TreeViewItem treeItem = trvBOM.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                if (treeItem != null)
                {
                    treeItem.IsExpanded = true;
                    foreach (var chlditm in treeItem.Items)
                    {
                        TreeViewItem chldtreeItem = trvBOM.ItemContainerGenerator.ContainerFromItem(chlditm) as TreeViewItem;
                        if (chldtreeItem != null)
                        {
                            chldtreeItem.IsExpanded = true;
                            expanded = true;
                        }
                    }
                }
            }
            if (expanded)
            {
                initTmr.Stop();
                initTmr.IsEnabled = false;
            }
        }

        private void trvBOM_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            MainViewModel vm = (MainViewModel)this.DataContext;
            vm.CurrentProject.SelectedComponent = trvBOM.SelectedItem as DAL.Component;
        }

        #region TreeView Drag-Drop with Visual Feedback

        private void TreeViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
            _isDragging = false;
        }

        private void TreeViewItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !_isDragging)
            {
                System.Windows.Point currentPosition = e.GetPosition(null);
                System.Windows.Vector diff = _startPoint - currentPosition;

                if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    TreeViewItem? treeViewItem = FindAncestor<TreeViewItem>((DependencyObject)e.OriginalSource);
                    if (treeViewItem != null)
                    {
                        _draggedItem = treeViewItem.DataContext as Component;
                        if (_draggedItem != null)
                        {
                            _isDragging = true;

                            // Create and show the drag adorner
                            CreateDragAdorner(treeViewItem, _draggedItem);

                            // Start drag operation
                            var result = System.Windows.DragDrop.DoDragDrop(treeViewItem, _draggedItem, System.Windows.DragDropEffects.Move);

                            // Clean up after drag
                            RemoveDragAdorner();
                            ClearHighlight();
                            _isDragging = false;
                            _draggedItem = null;
                        }
                    }
                }
            }
        }

        private void TreeViewItem_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = System.Windows.DragDropEffects.None;

            if (e.Data.GetDataPresent(typeof(Component)))
            {
                Component? targetItem = GetDropTarget(sender);
                Component? draggedItem = e.Data.GetData(typeof(Component)) as Component;

                TreeViewItem? targetTreeViewItem = sender as TreeViewItem;

                if (targetItem != null && draggedItem != null && targetTreeViewItem != null)
                {
                    bool isValidDrop = targetItem != draggedItem && !IsDescendant(draggedItem, targetItem);

                    if (isValidDrop)
                    {
                        e.Effects = System.Windows.DragDropEffects.Move;
                        HighlightDropTarget(targetTreeViewItem, true);
                    }
                    else
                    {
                        HighlightDropTarget(targetTreeViewItem, false);
                    }
                }

                // Update adorner position
                UpdateDragAdorner(e.GetPosition(trvBOM));
            }
            e.Handled = true;
        }

        private void TreeViewItem_DragLeave(object sender, DragEventArgs e)
        {
            if (sender is TreeViewItem treeViewItem)
            {
                // Reset the visual state when leaving
                treeViewItem.BorderBrush = System.Windows.Media.Brushes.Transparent;
                treeViewItem.Background = System.Windows.Media.Brushes.Transparent;
            }
            e.Handled = true;
        }

        private void TreeViewItem_Drop(object sender, DragEventArgs e)
        {
            ClearHighlight();

            if (e.Data.GetDataPresent(typeof(Component)))
            {
                Component? draggedItem = e.Data.GetData(typeof(Component)) as Component;
                Component? targetItem = GetDropTarget(sender);

                if (draggedItem != null && targetItem != null &&
                    draggedItem != targetItem && !IsDescendant(draggedItem, targetItem))
                {
                    var viewModel = DataContext as MainViewModel;
                    if (viewModel?.CurrentProject?.BOM?.Vehicle?.Components != null)
                    {
                        // Remove from original parent
                        RemoveFromParent(viewModel.CurrentProject.BOM.Vehicle.Components, draggedItem);

                        // Add to new parent
                        draggedItem.ParentID = targetItem.ID;
                        targetItem.AddComponent(draggedItem);
                    }
                }
            }
            e.Handled = true;
        }

        #endregion

        #region Visual Feedback Helpers

        private void CreateDragAdorner(TreeViewItem source, Component data)
        {
            _adornerLayer = AdornerLayer.GetAdornerLayer(trvBOM);
            if (_adornerLayer != null)
            {
                System.Windows.Point startPos = Mouse.GetPosition(trvBOM);
                _dragAdorner = new DragAdorner(trvBOM, data, startPos);
                _adornerLayer.Add(_dragAdorner);
            }
        }

        private void UpdateDragAdorner(System.Windows.Point position)
        {
            _dragAdorner?.UpdatePosition(position);
        }

        private void RemoveDragAdorner()
        {
            if (_adornerLayer != null && _dragAdorner != null)
            {
                _adornerLayer.Remove(_dragAdorner);
                _dragAdorner = null;
                _adornerLayer = null;
            }
        }

        private void HighlightDropTarget(TreeViewItem target, bool isValid)
        {
            // Clear previous highlight
            if (_lastHighlightedItem != null && _lastHighlightedItem != target)
            {
                _lastHighlightedItem.BorderBrush = System.Windows.Media.Brushes.Transparent;
                _lastHighlightedItem.Background = System.Windows.Media.Brushes.Transparent;
            }

            // Apply new highlight
            if (isValid)
            {
                target.BorderBrush = System.Windows.Media.Brushes.DodgerBlue;
                target.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(50, 51, 153, 255));
            }
            else
            {
                target.BorderBrush = System.Windows.Media.Brushes.Red;
                target.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(50, 255, 102, 102));
            }

            _lastHighlightedItem = target;
        }

        private void ClearHighlight()
        {
            if (_lastHighlightedItem != null)
            {
                _lastHighlightedItem.BorderBrush = System.Windows.Media.Brushes.Transparent;
                _lastHighlightedItem.Background = System.Windows.Media.Brushes.Transparent;
                _lastHighlightedItem = null;
            }
        }

        #endregion

        #region Drag-Drop Helper Methods

        private Component? GetDropTarget(object sender)
        {
            if (sender is TreeViewItem treeViewItem)
            {
                return treeViewItem.DataContext as Component;
            }
            return null;
        }

        private static T? FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T t)
                    return t;
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }

        private bool IsDescendant(Component parent, Component potentialChild)
        {
            foreach (var child in parent.Components)
            {
                if (child == potentialChild || IsDescendant(child, potentialChild))
                    return true;
            }
            return false;
        }

        private bool RemoveFromParent(System.Collections.ObjectModel.ObservableCollection<Component> components, Component itemToRemove)
        {
            if (components.Contains(itemToRemove))
            {
                components.Remove(itemToRemove);
                return true;
            }

            foreach (var component in components)
            {
                if (RemoveFromParent(component.Components, itemToRemove))
                    return true;
            }
            return false;
        }

        #endregion

        #region Image Drop Handlers

        private void Image_Drop(object sender, System.Windows.DragEventArgs e)
        {
            try
            {
                // 1. Handling Windows Explorer (FileDrop)
                if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
                {
                    string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
                    ProjImg.Source = new BitmapImage(new Uri(files[0]));
                }
                // 2. Handling Web Browsers (Images from Chrome/Edge)
                else if (e.Data.GetDataPresent(System.Windows.DataFormats.Bitmap))
                {
                    // Some browsers provide the actual Bitmap object
                    ProjImg.Source = (BitmapSource)e.Data.GetData(System.Windows.DataFormats.Bitmap);
                }
                // 3. Handling Web Browsers (HTML/URL)
                else if (e.Data.GetDataPresent(System.Windows.DataFormats.Text))
                {
                    string url = (string)e.Data.GetData(System.Windows.DataFormats.Text);
                    if (url.StartsWith("http"))
                    {
                        ProjImg.Source = new BitmapImage(new Uri(url));
                    }
                }
            }
            catch
            { }
            e.Handled = true; // Mark the event as handled
        }

        private void Image_DragOver(object sender, System.Windows.DragEventArgs e)
        {
            // Check if the dragged data contains file paths
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                // Set the effect to Copy, indicating a drop is allowed
                e.Effects = System.Windows.DragDropEffects.Copy;
            }
            else if (e.Data.GetDataPresent(System.Windows.DataFormats.Bitmap))
            {
                // Also allow if the data is a bitmap directly (e.g. from a web browser)
                e.Effects = System.Windows.DragDropEffects.Copy;
            }
            else
            {
                // Otherwise, disable the drop
                e.Effects = System.Windows.DragDropEffects.None;
            }
            e.Handled = true; // Mark the event as handled
        }

        #endregion

        private void CompImg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                // Your double-click logic here
                ImageView prv = new ImageView();
                prv.imgPanel.Source = CompImg.Source;
                prv.ShowDialog();
            }
        }
    }
}