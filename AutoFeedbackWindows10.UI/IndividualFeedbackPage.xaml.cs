﻿using AutoFeedbackWindows10.UI.DataProviders;
using AutoFeedbackWindows10.UI.Models;
using Microsoft.Toolkit.Uwp.UI.Animations.Expressions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AutoFeedbackWindows10.UI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IndividualFeedbackPage : Page
    {
        public ObservableCollection<FeedbackTeacherModel> Teachers { get; set; } = new ObservableCollection<FeedbackTeacherModel>();
        private FeedbackEntryProvider Provider => ((App)Application.Current).FeedbackProvider;

        public IndividualFeedbackPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var propSet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollRoot);
            var compositor = propSet.Compositor;
            var props = compositor.CreatePropertySet();
            props.InsertScalar("progress", 0);
            props.InsertScalar("clampSize", 150);

            var propScroll = propSet.GetSpecializedReference<ManipulationPropertySetReferenceNode>();
            var propGet = props.GetReference();
            var progressionNode = propGet.GetScalarProperty("progress");
            var clampSizeNode = propGet.GetScalarProperty("clampSize");

            ExpressionNode progressAnim = ExpressionFunctions.Clamp(-propScroll.Translation.Y / clampSizeNode, 0, 1);
            props.StartAnimation("progress", progressAnim);

            ExpressionNode opacityNode = ExpressionFunctions.Lerp(0, 1, progressionNode);
            ElementCompositionPreview.GetElementVisual(fadeBlackHeader).StartAnimation("Opacity", opacityNode);
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            lsvTeachers.PrepareConnectedAnimation("individualClick", e.ClickedItem, "templatePS");
            Frame.Navigate(typeof(IndividualFeedbackFormPage), e.ClickedItem, new SuppressNavigationTransitionInfo());
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var ca = ConnectedAnimationService.GetForCurrentView().GetAnimation("individualClickBack");
            if(ca != null)
                await lsvTeachers.TryStartConnectedAnimationAsync(ca, e.Parameter, "templateRoot");

            // Check if user has login
            if(Provider.CurrentAccount == null)
            {
                Frame.Navigate(typeof(LoginForm), null, new DrillInNavigationTransitionInfo());
                return;
            }
            // Add feedback entries
            await Task.Delay(100);
            prEntriesLoading.IsActive = true;
            await RefreshFeedbackEntries();
            prEntriesLoading.IsActive = false;
        }

        public async Task RefreshFeedbackEntries()
        {
            Teachers.Clear();
            foreach (var item in await Provider.GetFeedbackEntries())
            {
                Teachers.Add(item);
            }
        }

        public void Test(int thisWouldBeALongPathAsWellToA, int thisWouldBeALongPathAsWellToB, int thisWouldBeALongPathAsWellToC, int thisWouldBeALongPathAsWellToD)
        {
            List<Test> sources = new List<Test>();
            var filters = sources.Where(x =>
            (x.AVeryLongAccessorPathToA == thisWouldBeALongPathAsWellToA || thisWouldBeALongPathAsWellToA == -1) &&
            (x.AVeryLongAccessorPathToB == thisWouldBeALongPathAsWellToB || thisWouldBeALongPathAsWellToB == -1) &&
            (x.AVeryLongAccessorPathToC == thisWouldBeALongPathAsWellToC || thisWouldBeALongPathAsWellToC == -1) &&
            (x.AVeryLongAccessorPathToD == thisWouldBeALongPathAsWellToB || thisWouldBeALongPathAsWellToD == -1));
        }
    }

    class Test
    {
        public int AVeryLongAccessorPathToA { get; set; }
        public int AVeryLongAccessorPathToB { get; set; }
        public int AVeryLongAccessorPathToC { get; set; }
        public int AVeryLongAccessorPathToD { get; set; }
    }
}
