﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Minitrello.Phone.Models;

namespace Minitrello.Phone.ViewModels
{
    public class ItemViewModel : INotifyPropertyChanged
    {
        private string _id;
        /// <summary>
        /// Sample ViewModel property; this property is used to identify the object.
        /// </summary>
        /// <returns></returns>
        public string ID
        {
            get
            {
                return _id;
            }
            set
            {
                if (value != _id)
                {
                    _id = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        private string _lineOne;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public string LineOne
        {
            get
            {
                return _lineOne;
            }
            set
            {
                if (value != _lineOne)
                {
                    _lineOne = value;
                    NotifyPropertyChanged("LineOne");
                }
            }
        }

        private string _lineTwo;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public string LineTwo
        {
            get
            {
                return _lineTwo;
            }
            set
            {
                if (value != _lineTwo)
                {
                    _lineTwo = value;
                    NotifyPropertyChanged("LineTwo");
                }
            }
        }

        private string _lineThree;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public string LineThree
        {
            get
            {
                return _lineThree;
            }
            set
            {
                if (value != _lineThree)
                {
                    _lineThree = value;
                    NotifyPropertyChanged("LineThree");
                }
            }
        }
        private List<LaneModel> _lanes;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public List<LaneModel> Lanes
        {
            get
            {
                return _lanes;
            }
            set
            {
                if (value != _lanes)
                {
                    _lanes = value;
                    NotifyPropertyChanged("Lanes");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}