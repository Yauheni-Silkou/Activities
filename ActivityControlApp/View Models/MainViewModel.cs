using System.Collections.ObjectModel;
using System.Windows;

namespace ActivityControlApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private Order _order;
        private string _header;

        private readonly DataProvider _dataProvider = new();

        private ObservableCollection<User> _users;
        private User _selectedUser;

        private RelayCommand _loadCommand;
        private RelayCommand _loadAllCommand;
        private RelayCommand _exportCommand;
        private RelayCommand _exportAllCommand;

        private enum Order
        {
            Ascending,
            Descending
        }

        public ObservableCollection<User> Users
        {
            get { return _users; }
            set
            {
                if (_users != value)
                {
                    _users = value;
                    OnPropertyChanged();
                }
            }
        }

        public User SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                if (_selectedUser != value)
                {
                    _selectedUser = value;
                    OnPropertyChanged();
                }
            }
        }

        public Func<double, string> Formatter => value =>
        {
            int day = SelectedUser.Steps[(int)value].Day;
            return $"day {day}";
        };

        public RelayCommand LoadCommand
        {
            get
            {
                return _loadCommand ??= new RelayCommand(
                execute: _ =>
                {
                    string log = string.Empty;

                    Microsoft.Win32.OpenFileDialog dialog = new()
                    {
                        Multiselect = true,
                        Filter = "JSON data files|*.json",
                        Title = "Select data files"
                    };

                    bool? result = dialog.ShowDialog();

                    if (result is true)
                    {
                        _dataProvider.ReadData(dialog.FileNames,
                            (fileName, message) => AddToLog(ref log, fileName, message));
                        Users = new ObservableCollection<User>(_dataProvider.Users);
                    }

                    WriteLog(log);
                });
            }
        }

        public RelayCommand LoadAllCommand
        {
            get
            {
                return _loadAllCommand ??= new RelayCommand(
                execute: parameter =>
                {
                    string log = string.Empty;

                    using var dialog = new System.Windows.Forms.FolderBrowserDialog();
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        var filesList = Directory.GetFiles(dialog.SelectedPath)
                            .Where(file => file?.Length > 5
                            && string.Compare(file[^5..], ".json", ignoreCase: true) == 0);
                        _dataProvider.ReadData(filesList,
                            (fileName, message) => AddToLog(ref log, fileName, message));
                        Users = new ObservableCollection<User>(_dataProvider.Users);
                    }
                });
            }
        }

        public RelayCommand ExportCommand
        {
            get
            {
                return _exportCommand ??= new RelayCommand(
                execute: ext =>
                {
                    DataWriter writer = ext.ToString() switch
                    {
                        "json" => new JsonDataWriter(),
                        "xml" => new XmlDataWriter(),
                        _ => new CsvDataWriter()
                    };

                    string log = string.Empty;

                    using var dialog = new System.Windows.Forms.FolderBrowserDialog();
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        string path = dialog.SelectedPath;
                        _dataProvider.WriteData(SelectedUser, path, writer,
                            (fileName, message) => AddToLog(ref log, fileName, message));
                    }

                    WriteLog(log);
                },
                canExecute: _ => SelectedUser is not null);
            }
        }

        public RelayCommand ExportAllCommand
        {
            get
            {
                return _exportAllCommand ??= new RelayCommand(
                execute: ext =>
                {
                    DataWriter writer = ext.ToString() switch
                    {
                        "json" => new JsonDataWriter(),
                        "xml" => new XmlDataWriter(),
                        _ => new CsvDataWriter()
                    };

                    string log = string.Empty;

                    using var dialog = new System.Windows.Forms.FolderBrowserDialog();
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        string path = dialog.SelectedPath;
                        _dataProvider.WriteAllData(path, writer,
                            (fileName, message) => AddToLog(ref log, fileName, message));
                    }

                    WriteLog(log);
                },
                canExecute: _ => Users is not null && Users.Count > 0);
            }
        }

        public RelayCommand SortCommand
        {
            get
            {
                return new RelayCommand(
                execute: parameter =>
                {
                    string value = parameter.ToString();
                    
                    if (Users is null)
                    {
                        return;
                    }

                    if (_header == value)
                    {
                        switch (_order)
                        {
                            case Order.Ascending: _order = Order.Descending; break;
                            case Order.Descending: _order = Order.Ascending; break;
                        }
                    }
                    else
                    {
                        _header = value;
                    }

                    if (_order == Order.Descending)
                    {
                        switch (_header)
                        {
                            case "Name":   Users = new ObservableCollection<User>(Users.OrderByDescending(x => x.Name)); break;
                            case "Status": Users = new ObservableCollection<User>(Users.OrderByDescending(x => x.Status)); break;
                            case "Mean":   Users = new ObservableCollection<User>(Users.OrderByDescending(x => x.Mean)); break;
                            case "Max":    Users = new ObservableCollection<User>(Users.OrderByDescending(x => x.Max)); break;
                            case "Min":    Users = new ObservableCollection<User>(Users.OrderByDescending(x => x.Min)); break;
                        }
                    }
                    else
                    {
                        switch (_header)
                        {
                            case "Name":   Users = new ObservableCollection<User>(Users.OrderBy(x => x.Name)); break;
                            case "Status": Users = new ObservableCollection<User>(Users.OrderBy(x => x.Status)); break;
                            case "Mean":   Users = new ObservableCollection<User>(Users.OrderBy(x => x.Mean)); break;
                            case "Max":    Users = new ObservableCollection<User>(Users.OrderBy(x => x.Max)); break;
                            case "Min":    Users = new ObservableCollection<User>(Users.OrderBy(x => x.Min)); break;
                        }
                    }

                });
            }
        }
        
        private static void AddToLog(ref string log, string fileName, string message)
        {
            log += $"File\n\n{fileName} message\n\n";
            log += $"Error message\n\n{message} message\n\n\n\n";
        }

        private static void WriteLog(string log)
        {
            if (log != string.Empty)
            {
                MessageBox.Show("Some errors have occured! You can save an error log or refuse.");
                Microsoft.Win32.SaveFileDialog saveDialog = new()
                {
                    Title = "Save error log",
                    Filter = "Text file|*.txt",
                    FileName = "log"
                };

                bool? saveResult = saveDialog.ShowDialog();

                if (saveResult is true)
                {
                    try
                    {
                        using StreamWriter stream = new(saveDialog.FileName, false, Encoding.UTF8);
                        stream.Write(log);
                    }
                    catch
                    {
                        MessageBox.Show("Unable to save error log.");
                    }
                }
            }
        }
    }
}
