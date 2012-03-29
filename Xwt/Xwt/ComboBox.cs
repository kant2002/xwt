// 
// ComboBox.cs
//  
// Author:
//       Lluis Sanchez <lluis@xamarin.com>
// 
// Copyright (c) 2011 Xamarin Inc
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using Xwt.Backends;

namespace Xwt
{
	public class ComboBox: Widget
	{
		CellViewCollection views;
		IListDataSource source;
		ItemCollection itemCollection;
		
		protected new class EventSink: Widget.EventSink, IComboBoxEventSink, ICellContainer
		{
			public void NotifyCellChanged ()
			{
				((ComboBox)Parent).OnCellChanged ();
			}
			
			public void OnSelectionChanged ()
			{
				((ComboBox)Parent).OnSelectionChanged (EventArgs.Empty);
			}
			
			public bool RowIsSeparator (int rowIndex)
			{
				return ((ComboBox)Parent).RowIsSeparator (rowIndex);
			}
			
			public override Size GetDefaultNaturalSize ()
			{
				return Xwt.Engine.DefaultNaturalSizes.ComboBox;
			}
		}
		
		new IComboBoxBackend Backend {
			get { return (IComboBoxBackend) base.Backend; }
		}
		
		public ComboBox ()
		{
			views = new CellViewCollection ((ICellContainer)WidgetEventSink);
		}
		
		protected override Widget.EventSink CreateEventSink ()
		{
			return new EventSink ();
		}
		
		public CellViewCollection Views {
			get { return views; }
		}
		
		public ItemCollection Items {
			get {
				if (itemCollection == null) {
					itemCollection = new ItemCollection ();
					ItemsSource = itemCollection.DataSource;
				} else {
					if (ItemsSource != itemCollection.DataSource)
						throw new InvalidOperationException ("The Items collection can't be used when a custom DataSource is set");
				}
				return itemCollection;
			}
		}
		
		public IListDataSource ItemsSource {
			get { return source; }
			set {
				if (source != null) {
					source.RowChanged -= HandleModelChanged;
					source.RowDeleted -= HandleModelChanged;
					source.RowInserted -= HandleModelChanged;
					source.RowsReordered -= HandleModelChanged;
				}
				
				source = value;
				Backend.SetSource (source, source is XwtComponent ? GetBackend ((XwtComponent)source) : null);
				
				if (source != null) {
					source.RowChanged += HandleModelChanged;
					source.RowDeleted += HandleModelChanged;
					source.RowInserted += HandleModelChanged;
					source.RowsReordered += HandleModelChanged;
				}
			}
		}

		void HandleModelChanged (object sender, ListRowEventArgs e)
		{
			OnPreferredSizeChanged ();
		}
		
		public int SelectedIndex {
			get { return Backend.SelectedRow; }
			set { Backend.SelectedRow = value; }
		}
		
		public object SelectedItem {
			get {
				if (Backend.SelectedRow == -1)
					return null;
				return Items [Backend.SelectedRow];
			}
			set {
				SelectedIndex = Items.IndexOf (value);
			}
		}
		
		public object SelectedText {
			get {
				if (Backend.SelectedRow == -1)
					return null;
				return Items [Backend.SelectedRow];
			}
			set {
				SelectedIndex = Items.IndexOf (value);
			}
		}
		
		public Func<int,bool> RowSeparatorCheck {
			get; set;
		}
		
		void OnCellChanged ()
		{
			Backend.SetViews (views);
		}
		
		EventHandler selectionChanged;
		
		public event EventHandler SelectionChanged {
			add {
				OnBeforeEventAdd (ComboBoxEvent.SelectionChanged, selectionChanged);
				selectionChanged += value;
			}
			remove {
				selectionChanged -= value;
				OnAfterEventRemove (ComboBoxEvent.SelectionChanged, selectionChanged);
			}
		}
		
		protected virtual void OnSelectionChanged (EventArgs args)
		{
			if (selectionChanged != null)
				selectionChanged (this, args);
		}
		
		protected virtual bool RowIsSeparator (int rowIndex)
		{
			if (RowSeparatorCheck != null)
				return RowSeparatorCheck (rowIndex);
			if (itemCollection != null && itemCollection.DataSource == source)
				return Items [rowIndex] is ItemSeparator;
			else
				return false;
		}
	}
}
