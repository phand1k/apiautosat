namespace AvtoMigBussines.Exceptions
{
    public class CustomException : Exception
    {
        public class UserAlreadyExistsException : Exception
        {
            public UserAlreadyExistsException(string message) : base(message)
            {
            }
        }
        public class OrganizationNotFoundException : Exception
        {
            public OrganizationNotFoundException(string message) : base(message)
            {
            }
        }
        public class WashOrderNotFoundException : Exception
        {
            public WashOrderNotFoundException(string message) : base(message) { }
        }

        public class OrganizationExistsException : Exception
        {
            public OrganizationExistsException(string message) : base(message)
            {

            }
        }
        public class WashOrderExistsException : Exception
        {
            public WashOrderExistsException(string message) : base(message)
            {

            }
        }
        public class UserNotFoundException : Exception
        {
            public UserNotFoundException(string message) : base(message)
            {

            }
        }

        public class ServiceExistsException : Exception
        {
            public ServiceExistsException(string message) : base(message)
            {

            }
        }
    }
}
