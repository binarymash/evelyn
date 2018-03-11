namespace Evelyn.Core
{
    using System;

    public static class Constants
    {
        public static Guid EvelynSystem => Guid.Parse("BE9CBFA9-5E75-4F03-B240-B8F6C3C61533");

        public static string SystemUser => "SystemUser";

        public static Guid DefaultAccount => Guid.Parse("E70FD009-22C4-44E0-AB13-2B6EDAF0BBDB");

        public static string AnonymousUser => "AnonymousUser";
    }
}
