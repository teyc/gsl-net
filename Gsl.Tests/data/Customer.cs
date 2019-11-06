namespace CodeGen.Demo
{
    class Customer
    {
        public string FirstName
        {
            get
            {
                // CUSTOM-CODE-BEGIN get-FirstName
                /* 12-May-2018 Note there there are no validation rules in the DB */
                return _firstName;
                // CUSTOM-CODE-END get-FirstName
            }
        }
        public string LastName
        {
            get
            {
                // CUSTOM-CODE-BEGIN get-LastName
                return _lastName;
                // CUSTOM-CODE-END get-LastName
            }
        }
    }
}