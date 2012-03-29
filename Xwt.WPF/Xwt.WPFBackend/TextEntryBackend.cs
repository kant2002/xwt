﻿// 
// TextEntryBackend.cs
//  
// Author:
//       Eric Maupin <ermau@xamarin.com>
// 
// Copyright (c) 2012 Xamarin, Inc.
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Xwt.Backends;
using Xwt.WPFBackend.Utilities;

namespace Xwt.WPFBackend
{
	public class TextEntryBackend
		: WidgetBackend, ITextEntryBackend
	{
		public TextEntryBackend()
		{
			Widget = new ExTextBox();
		}

		public virtual string Text
		{
			get { return TextBox.Text; }
			set { TextBox.Text = value; }
		}

		public string PlaceholderText
		{
			get { return TextBox.PlaceholderText; }
			set { TextBox.PlaceholderText = value; }
		}

		public bool ReadOnly
		{
			get { return TextBox.IsReadOnly; }
			set { TextBox.IsReadOnly = true; }
		}

		public bool ShowFrame
		{
			get { return TextBox.ShowFrame; }
			set { TextBox.ShowFrame = value; }
		}

		public override void EnableEvent (object eventId)
		{
			base.EnableEvent (eventId);

			if (eventId is TextEntryEvent)
			{
				switch ((TextEntryEvent)eventId)
				{
					// TODO: Should we ignore this for placeholder changes?
					case TextEntryEvent.Changed:
						TextBox.TextChanged += OnTextChanged;
						break;
				}
			}
		}

		public override void DisableEvent (object eventId)
		{
			base.DisableEvent (eventId);

			if (eventId is TextEntryEvent)
			{
				switch ((TextEntryEvent)eventId)
				{
					case TextEntryEvent.Changed:
						TextBox.TextChanged -= OnTextChanged;
						break;
				}
			}
		}

		protected ExTextBox TextBox
		{
			get { return (ExTextBox) Widget; }
		}

		protected new ITextEntryEventSink EventSink {
			get { return (ITextEntryEventSink)base.EventSink; }
		}

		private void OnTextChanged (object s, TextChangedEventArgs e)
		{
			Xwt.Engine.Toolkit.Invoke (EventSink.OnChanged);
		}
	}
}