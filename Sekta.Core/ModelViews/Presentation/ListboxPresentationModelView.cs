using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using Sekta.Admx.Schema;
using Sekta.Core.Schema;

namespace Sekta.Core.ModelView.Presentation
{
    public class ListboxItemPresentationModelView : ReactiveObject
    {
        private string _keyName;

        public string KeyName
        {
            get { return _keyName; }
            set { this.RaiseAndSetIfChanged(ref _keyName, value); }
        }

        private string _value;

        public string Value
        {
            get { return _value; }
            set { this.RaiseAndSetIfChanged(ref _value, value); }
        }
    }


    public class ListboxPresentationModelView : BasePresentationModeView
    {
        private readonly AdmxPolicy _admxPolicy;
        private ListBox _presentationElement;
        private ListElement _enumerationElement;

        private readonly SourceList<ListboxItemPresentationModelView> _itemList;
        private readonly ReadOnlyObservableCollection<ListboxItemPresentationModelView> _items;

        public ReactiveCommand<Unit, Unit> AddItemCommand { get; }

        public ReactiveCommand<ListboxItemPresentationModelView, Unit> RemoveItemCommand { get; }

        public ListboxPresentationModelView(ListBox presentationElement, AdmxPolicy admxPolicy)
        {
            _admxPolicy = admxPolicy;
            _itemList = new SourceList<ListboxItemPresentationModelView>();
            _itemList
                .Connect()
                .AutoRefresh()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _items)
                .Subscribe();

            PresentationElement = presentationElement;

            RevertToDefaultValues();

            AddItemCommand = ReactiveCommand.Create(AddItem);
            RemoveItemCommand = ReactiveCommand.Create<ListboxItemPresentationModelView>(RemoveItem);

            ValidationContext = new ValidationContext();


            var itemsChanged = this.Items.ToObservableChangeSet().ToCollection().Select(
                (cs) => cs.All((i) => !string.IsNullOrWhiteSpace(i.Value) && !string.IsNullOrWhiteSpace(i.KeyName)));

            this.ValidationRule(
                (vm) => vm.Items,
                itemsChanged,
                "Both key name and value are required."
            );
        }

        private void RemoveItem(ListboxItemPresentationModelView item)
        {
            if (item != null)
            {
                _itemList.Remove(item);
            }
        }

        private void AddItem()
        {
            _itemList.Add(new ListboxItemPresentationModelView());
        }

        public ReadOnlyObservableCollection<ListboxItemPresentationModelView> Items => _items;

        public ListBox PresentationElement
        {
            get => _presentationElement;
            set => this.RaiseAndSetIfChanged(ref _presentationElement, value);
        }

        public ListElement EnumerationElement
        {
            get { return _enumerationElement; }
            set { this.RaiseAndSetIfChanged(ref _enumerationElement, value); }
        }

        public override PolicyOptionValue Serialize(BaseElement[] elements)
        {
            var id = PresentationElement.RefId;

            return new PolicyOptionValue(EnumerationElement.key ?? _admxPolicy.Key, EnumerationElement.valuePrefix,
                Items.Select((it) => new KeyValuePair<string, string>(it.KeyName, it.Value)).ToArray(),
                EnumerationElement.id);
        }

        public override void Deserialize(BaseElement[] elements, PolicyOptionValue? serializedValue)
        {
            EnumerationElement = (ListElement)elements.First((e) => e.id == _presentationElement.RefId);

            if (serializedValue == null)
            {
                RevertToDefaultValues();
            }
            else
            {
                _itemList.Edit((innerList) =>
                {
                    innerList.Clear();
                    innerList.AddRange(serializedValue.Value.ToStringListValue().Select((kv) =>
                        new ListboxItemPresentationModelView()
                        {
                            KeyName = kv.Key,
                            Value = kv.Value
                        }));
                });
            }
        }

        public override void RevertToDefaultValues()
        {
            _itemList.Edit((innerList) =>
            {
                innerList.Clear();
                innerList.Add(new ListboxItemPresentationModelView());
            });
        }

        public override string Id => _presentationElement.RefId;

        public override bool HasRelevantDataChanged(string propertyName)
        {
            return propertyName == nameof(Items);
        }
    }
}