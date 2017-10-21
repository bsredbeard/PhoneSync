using PhoneSync.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhoneSync.ViewModels
{
    public class MainScreenVM : ObservableObject, IDisposable
    {
        private readonly SynchronizationContext _syncContext = SynchronizationContext.Current;
        private readonly DeviceExplorer _explorer = new DeviceExplorer();

        private readonly ObservableCollection<string> _drives = new ObservableCollection<string>();
        private readonly ObservableCollection<TransferInfo> _driveItems = new ObservableCollection<TransferInfo>();
        private ICommand _exploreSelectedDrive;
        private ICommand _refreshDriveList;
        private string _selectedDrive = null;
        private bool _isScanning = false;

        public MainScreenVM()
        {
            ExploreSelectedDrive = CreateCommand(DoExplore, () => !string.IsNullOrWhiteSpace(SelectedDrive), nameof(SelectedDrive));
            RefreshDriveList = CreateCommand(DoRefresh, () => !IsScanning, nameof(IsScanning));

            Drives.Add("Hey");
            Drives.Add("you");
            Drives.Add("get into my car");
        }

        /// <summary>
        /// The collection of drives
        /// </summary>
        public ObservableCollection<string> Drives { get { return _drives; } }

        /// <summary>
        /// The items in the selected drive
        /// </summary>
        public ObservableCollection<TransferInfo> DriveItems { get { return _driveItems; } }

        /// <summary>
        /// The index of the selected drive
        /// </summary>
        public string SelectedDrive
        {
            get { return _selectedDrive; }
            set
            {
                if (value != _selectedDrive)
                {
                    _selectedDrive = value;
                    RaiseChange(nameof(SelectedDrive));
                }
            }
        }

        /// <summary>
        /// If the viewmodel is disposed
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// If the app is doing a scanning operation
        /// </summary>
        public bool IsScanning
        {
            get { return _isScanning; }
            set
            {
                _isScanning = value;
                RaiseChange(nameof(IsScanning));
            }
        }

        /// <summary>
        /// A command to refresh the drive list
        /// </summary>
        public ICommand RefreshDriveList
        {
            get { return _refreshDriveList; }
            set
            {
                _refreshDriveList = value;
                RaiseChange(nameof(RefreshDriveList));
            }
        }

        /// <summary>
        /// The command to explore the currently selected drive
        /// </summary>
        public ICommand ExploreSelectedDrive
        {
            get { return _exploreSelectedDrive; }
            set
            {
                _exploreSelectedDrive = value;
                RaiseChange(nameof(ExploreSelectedDrive));
            }
        }

        private void DoRefresh(object arg)
        {
            IsScanning = true;
            _explorer.GetDrives().SyncTo(_syncContext, drives =>
            {
                SelectedDrive = null;
                Drives.Clear();
                drives.ForEach(item => Drives.Add(item));
                IsScanning = false;
            });
        }

        private void DoExplore(object arg)
        {
            IsScanning = true;
            _explorer.TransferFiles(SelectedDrive).SyncTo(_syncContext, items =>
            {
                DriveItems.Clear();
                items.ForEach(x => DriveItems.Add(x));
                IsScanning = false;
            });
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                _explorer.Dispose();
                Drives.Clear();
                DriveItems.Clear();
            }
        }
    }
}
