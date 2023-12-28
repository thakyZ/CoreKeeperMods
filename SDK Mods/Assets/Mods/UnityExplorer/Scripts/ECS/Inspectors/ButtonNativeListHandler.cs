using System;
using System.Collections.Generic;
using Unity.Collections;
using UniverseLib.UI.Widgets.ButtonList;
using UniverseLib.UI.Widgets.ScrollView;

namespace ECSExtension
{
    public class ButtonNativeListHandler<TData, TCell> : ICellPoolDataSource<TCell> where TCell : ButtonCell where TData : unmanaged
    {
        protected Func<NativeArray<TData>> GetEntries;
        protected Action<TCell, int> SetICell;
        protected Func<TData, string, bool> ShouldDisplay;
        protected Action<int> OnCellClicked;
        private string currentFilter;

        public ScrollPool<TCell> ScrollPool { get; private set; }

        public int ItemCount => CurrentEntries.Count;

        public List<TData> CurrentEntries { get; } = new List<TData>();

        public string CurrentFilter
        {
            get => currentFilter;
            set => currentFilter = value ?? "";
        }

        /// <summary>Create a wrapper to handle your Button ScrollPool.</summary>
        /// <param name="scrollPool">The ScrollPool&lt;ButtonCell&gt; you have already created.</param>
        /// <param name="getEntriesMethod">A method which should return your current data values.</param>
        /// <param name="setICellMethod">A method which should set the data at the int index to the cell.</param>
        /// <param name="shouldDisplayMethod">A method which should determine if the data at the index should be displayed, with an optional string filter from CurrentFilter.</param>
        /// <param name="onCellClickedMethod">A method invoked when a cell is clicked, containing the data index assigned to the cell.</param>
        public ButtonNativeListHandler(
            ScrollPool<TCell> scrollPool,
            Func<NativeArray<TData>> getEntriesMethod,
            Action<TCell, int> setICellMethod,
            Func<TData, string, bool> shouldDisplayMethod,
            Action<int> onCellClickedMethod)
        {
            ScrollPool = scrollPool;
            GetEntries = getEntriesMethod;
            SetICell = setICellMethod;
            ShouldDisplay = shouldDisplayMethod;
            OnCellClicked = onCellClickedMethod;
        }

        public void RefreshData()
        {
            NativeArray<TData> dataList = GetEntries();
            CurrentEntries.Clear();
            foreach (TData data in dataList)
            {
                if (!string.IsNullOrEmpty(currentFilter))
                {
                    if (ShouldDisplay(data, currentFilter))
                        CurrentEntries.Add(data);
                }
                else
                    CurrentEntries.Add(data);
            }
        }

        public virtual void OnCellBorrowed(TCell cell)
        {
            cell.OnClick += OnCellClicked;
        }

        public virtual void SetCell(TCell cell, int index)
        {
            if (CurrentEntries == null)
                RefreshData();
            if (index < 0 || index >= CurrentEntries.Count)
            {
                cell.Disable();
            }
            else
            {
                cell.Enable();
                cell.CurrentDataIndex = index;
                SetICell(cell, index);
            }
        }
    }
}