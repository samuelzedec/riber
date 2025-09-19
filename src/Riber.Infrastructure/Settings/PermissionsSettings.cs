namespace Riber.Infrastructure.Settings;

public static class PermissionsSettings
{
    public static class Companies
    {
        public const string Read = "companies.read";
        public const string Update = "companies.update";
        public const string Delete = "companies.delete";
        public const string ManageUsers = "companies.manage_users";
    }

    public static class Orders
    {
        public const string Create = "orders.create";
        public const string Read = "orders.read";
        public const string Update = "orders.update";
        public const string Delete = "orders.delete";
    }

    public static class Products
    {
        public const string Create = "products.create";
        public const string Read = "products.read";
        public const string Update = "products.update";
        public const string Delete = "products.delete";
        public const string Import = "products.import";
    }

    public static class Categories
    {
        public const string Create = "categories.create";
        public const string Read = "categories.read";
        public const string Update = "categories.update";
        public const string Delete = "categories.delete";
    }

    public static class Users
    {
        public const string Create = "users.create";
        public const string Read = "users.read";
        public const string Update = "users.update";
        public const string Delete = "users.delete";
        public const string AssignRoles = "users.assign_roles";
    }

    public static class Reports
    {
        public const string View = "reports.view";
        public const string Export = "reports.export";
        public const string Schedule = "reports.schedule";
    }

    public static class Settings
    {
        public const string View = "settings.view";
        public const string Update = "settings.update";
    }

    public static class Roles
    {
        public const string Read = "roles.read";
        public const string Update = "roles.update";
    }
}