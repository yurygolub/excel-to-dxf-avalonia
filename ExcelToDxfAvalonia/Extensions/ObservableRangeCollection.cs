using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ExcelToDxfAvalonia.Extensions;

public class ObservableRangeCollection<T> : ObservableCollection<T>
{
    public ObservableRangeCollection()
        : base()
    {
    }

    public ObservableRangeCollection(IEnumerable<T> collection)
        : base(collection)
    {
    }

    public ObservableRangeCollection(List<T> list)
        : base(list)
    {
    }

    public void AddRange(IEnumerable<T> range)
    {
        _ = range ?? throw new ArgumentNullException(nameof(range));

        foreach (var item in range)
        {
            this.Items.Add(item);
        }

        this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
        this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
}
