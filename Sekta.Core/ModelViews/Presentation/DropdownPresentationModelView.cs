using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using Sekta.Admx.Schema;
using Sekta.Core.Schema;

namespace Sekta.Core.ModelView.Presentation
{
    public class DropdownPresentationModelView: BasePresentationModeView
    {
        private readonly AdmxPolicy _admxPolicy;
        private DropdownList _presentationElement;
        private uint _selectedItemIndex;
        private EnumerationElement _enumerationElement;

        readonly ObservableAsPropertyHelper<string[]> _items;

        public DropdownPresentationModelView(DropdownList presentationElement, AdmxPolicy admxPolicy)
        {
            this._admxPolicy = admxPolicy;
            PresentationElement = presentationElement;

            RevertToDefaultValues();

            this.WhenAnyValue((vm) => vm.EnumerationElement, (vm) => vm.CurrentResources)
                .Where((x) => x.Item1 != null)
                .Select(((x, i) => x.Item1.item.Select((item) => x.Item2 == null ? item.displayName : item.displayName.LocalizeWith(x.Item2)).ToArray()))
                .ToProperty(this, (vm) => vm.Items, out _items);
        }

        public string[] Items => _items.Value;

        public uint SelectedItemIndex
        {
            get { return _selectedItemIndex; }
            set { this.RaiseAndSetIfChanged(ref _selectedItemIndex, value); }
        }

        public uint DefaultItemIndex
        {
            get { return _presentationElement.DefaultItem; }
        }

        public DropdownList PresentationElement
        {
            get => _presentationElement;
            set => this.RaiseAndSetIfChanged(ref _presentationElement, value);
        }

        public EnumerationElement EnumerationElement
        {
            get { return _enumerationElement; }
            set { this.RaiseAndSetIfChanged(ref _enumerationElement, value); }
        }

        public override PolicyOptionValue Serialize(BaseElement[] elements)
        {
            var id = PresentationElement.RefId;

            var selectedItem = EnumerationElement.item[SelectedItemIndex];

            return selectedItem.value.AsPolicyOption(EnumerationElement.key ?? _admxPolicy.Key, EnumerationElement.valueName, EnumerationElement.id);
        }

        public override void Deserialize(BaseElement[] elements, PolicyOptionValue? serializedValue)
        {
            EnumerationElement = (EnumerationElement) elements.First((e) => e.id == _presentationElement.RefId);

            if (serializedValue == null)
            {
                RevertToDefaultValues();
            }
            else
            {
                int index = EnumerationElement.item
                    .Zip(Enumerable.Range(0, EnumerationElement.item.Length), ((item, i) => (item, i)))
                    .First((itemTuple) => serializedValue.Value.HasSameValue(itemTuple.item.value)).i;

                SelectedItemIndex = index >= 0 ? (uint) index : DefaultItemIndex;
            }
        }

        public override void RevertToDefaultValues()
        {
            SelectedItemIndex = DefaultItemIndex;
        }

        public override string Id => _presentationElement.RefId;

        public override bool HasRelevantDataChanged(string propertyName)
        {
            return propertyName == nameof(SelectedItemIndex);
        }
    }
}
