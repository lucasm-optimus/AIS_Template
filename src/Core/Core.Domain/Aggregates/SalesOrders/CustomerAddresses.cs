namespace Tilray.Integrations.Core.Domain.Aggregates.SalesOrders
{
    public class CustomerAddresses : Entity
    {
        public Address Acknowledgement { get; private set; }
        public Address ShipTo { get; private set; }
        public Address Installation { get; private set; }
        public Address BillTo { get; private set; }

        private CustomerAddresses() { }

        public static CustomerAddresses Create(Address acknowledgement, Address shipTo, Address installation, Address billTo)
        {
            return new CustomerAddresses
            {
                Acknowledgement = acknowledgement,
                ShipTo = shipTo,
                Installation = installation,
                BillTo = billTo
            };
        }
    }


    public class Address : Entity
    {
        public AddressReference AddressReference { get; private set; }

        private Address() { }

        private Address(AddressReference addressReference)
        {
            AddressReference = addressReference;
        }

        public static Address Create(AddressReference addressReference)
        {
            return new Address(addressReference);
        }
    }



    public class AddressReference : Entity
    {
        public string Reference { get; private set; }

        private AddressReference() { }

        private AddressReference(string reference)
        {
            Reference = reference;
        }

        public static AddressReference Create(string reference)
        {
            return new AddressReference(reference);
        }
    }
}
