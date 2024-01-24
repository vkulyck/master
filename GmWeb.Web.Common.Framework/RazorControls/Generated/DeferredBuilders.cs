
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI.Fluent;

namespace GmWeb.Web.Common.RazorControls
{
    public partial class GmKendoFactory
    {
        public override ArcGaugeBuilder ArcGauge() => base.ArcGauge().Deferred();
        public override AutoCompleteBuilder AutoComplete() => base.AutoComplete().Deferred();
        public override BarcodeBuilder Barcode() => base.Barcode().Deferred();
        public override ButtonBuilder Button() => base.Button().Deferred();
        public override ButtonGroupBuilder ButtonGroup() => base.ButtonGroup().Deferred();
        public override CalendarBuilder Calendar() => base.Calendar().Deferred();
        public override ChatBuilder Chat() => base.Chat().Deferred();
        public override CheckBoxBuilder CheckBox() => base.CheckBox().Deferred();
        public override ColorPaletteBuilder ColorPalette() => base.ColorPalette().Deferred();
        public override ColorPickerBuilder ColorPicker() => base.ColorPicker().Deferred();
        public override ComboBoxBuilder ComboBox() => base.ComboBox().Deferred();
        public override ContextMenuBuilder ContextMenu() => base.ContextMenu().Deferred();
        public override DateInputBuilder DateInput() => base.DateInput().Deferred();
        public override DatePickerBuilder DatePicker() => base.DatePicker().Deferred();
        public override DateRangePickerBuilder DateRangePicker() => base.DateRangePicker().Deferred();
        public override DateTimePickerBuilder DateTimePicker() => base.DateTimePicker().Deferred();
        public override DialogBuilder Dialog() => base.Dialog().Deferred();
        public override DrawerBuilder Drawer() => base.Drawer().Deferred();
        public override DropDownListBuilder DropDownList() => base.DropDownList().Deferred();
        public override DropDownTreeBuilder DropDownTree() => base.DropDownTree().Deferred();
        public override EditorBuilder Editor() => base.Editor().Deferred();
        public override FlatColorPickerBuilder FlatColorPicker() => base.FlatColorPicker().Deferred();
        public override LinearGaugeBuilder LinearGauge() => base.LinearGauge().Deferred();
        public override ListBoxBuilder ListBox() => base.ListBox().Deferred();
        public override MapBuilder Map() => base.Map().Deferred();
        public override MaskedTextBoxBuilder MaskedTextBox() => base.MaskedTextBox().Deferred();
        public override MediaPlayerBuilder MediaPlayer() => base.MediaPlayer().Deferred();
        public override MenuBuilder Menu() => base.Menu().Deferred();
        public override MobileActionSheetBuilder MobileActionSheet() => base.MobileActionSheet().Deferred();
        public override MobileApplicationBuilder MobileApplication() => base.MobileApplication().Deferred();
        public override MobileBackButtonBuilder MobileBackButton() => base.MobileBackButton().Deferred();
        public override MobileButtonBuilder MobileButton() => base.MobileButton().Deferred();
        public override MobileButtonGroupBuilder MobileButtonGroup() => base.MobileButtonGroup().Deferred();
        public override MobileCollapsibleBuilder MobileCollapsible() => base.MobileCollapsible().Deferred();
        public override MobileDetailButtonBuilder MobileDetailButton() => base.MobileDetailButton().Deferred();
        public override MobileDrawerBuilder MobileDrawer() => base.MobileDrawer().Deferred();
        public override MobileLayoutBuilder MobileLayout() => base.MobileLayout().Deferred();
        public override MobileModalViewBuilder MobileModalView() => base.MobileModalView().Deferred();
        public override MobileNavBarBuilder MobileNavBar() => base.MobileNavBar().Deferred();
        public override MobilePopOverBuilder MobilePopOver() => base.MobilePopOver().Deferred();
        public override MobileScrollViewBuilder MobileScrollView() => base.MobileScrollView().Deferred();
        public override MobileSplitViewBuilder MobileSplitView() => base.MobileSplitView().Deferred();
        public override MobileSwitchBuilder MobileSwitch() => base.MobileSwitch().Deferred();
        public override MobileTabStripBuilder MobileTabStrip() => base.MobileTabStrip().Deferred();
        public override MobileViewBuilder MobileView() => base.MobileView().Deferred();
        public override MultiColumnComboBoxBuilder MultiColumnComboBox() => base.MultiColumnComboBox().Deferred();
        public override MultiSelectBuilder MultiSelect() => base.MultiSelect().Deferred();
        public override MultiViewCalendarBuilder MultiViewCalendar() => base.MultiViewCalendar().Deferred();
        public override NotificationBuilder Notification() => base.Notification().Deferred();
        public override PanelBarBuilder PanelBar() => base.PanelBar().Deferred();
        public override PDFViewerBuilder PDFViewer() => base.PDFViewer().Deferred();
        public override PivotConfiguratorBuilder PivotConfigurator() => base.PivotConfigurator().Deferred();
        public override ProgressBarBuilder ProgressBar() => base.ProgressBar().Deferred();
        public override QRCodeBuilder QRCode() => base.QRCode().Deferred();
        public override RadialGaugeBuilder RadialGauge() => base.RadialGauge().Deferred();
        public override RadioButtonBuilder RadioButton() => base.RadioButton().Deferred();
        public override RecurrenceEditorBuilder RecurrenceEditor() => base.RecurrenceEditor().Deferred();
        public override ResponsivePanelBuilder ResponsivePanel() => base.ResponsivePanel().Deferred();
        public override ScrollViewBuilder ScrollView() => base.ScrollView().Deferred();
        public override SortableBuilder Sortable() => base.Sortable().Deferred();
        public override SplitterBuilder Splitter() => base.Splitter().Deferred();
        public override SpreadsheetBuilder Spreadsheet() => base.Spreadsheet().Deferred();
        public override SwitchBuilder Switch() => base.Switch().Deferred();
        public override TabStripBuilder TabStrip() => base.TabStrip().Deferred();
        public override TimePickerBuilder TimePicker() => base.TimePicker().Deferred();
        public override TimezoneEditorBuilder TimezoneEditor() => base.TimezoneEditor().Deferred();
        public override ToolBarBuilder ToolBar() => base.ToolBar().Deferred();
        public override TooltipBuilder Tooltip() => base.Tooltip().Deferred();
        public override TreeMapBuilder TreeMap() => base.TreeMap().Deferred();
        public override TreeViewBuilder TreeView() => base.TreeView().Deferred();
        public override UploadBuilder Upload() => base.Upload().Deferred();
        public override WindowBuilder Window() => base.Window().Deferred();
        public new TreeListBuilder<T> TreeList<T>() where T : class => base.TreeList<T>().Deferred();
        public new GridBuilder<T> Grid<T>() where T : class => base.Grid<T>().Deferred();
        public new ListViewBuilder<T> ListView<T>() where T : class => base.ListView<T>().Deferred();
        public new MobileListViewBuilder<T> MobileListView<T>() where T : class => base.MobileListView<T>().Deferred();
        public new PivotGridBuilder<T> PivotGrid<T>() where T : class => base.PivotGrid<T>().Deferred();
        public new TextBoxBuilder<T> TextBox<T>() where T : class => base.TextBox<T>().Deferred();
        public new ChartBuilder<T> Chart<T>() where T : class => base.Chart<T>().Deferred();
        public new StockChartBuilder<T> StockChart<T>() where T : class => base.StockChart<T>().Deferred();
        public new SparklineBuilder<T> Sparkline<T>() where T : class => base.Sparkline<T>().Deferred();
    }

}
