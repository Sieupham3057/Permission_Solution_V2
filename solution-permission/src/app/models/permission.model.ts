export type PermissionNames =
    'View Users' | 'Manage Users' |
    'View Roles' | 'Manage Roles' | 'Assign Roles' |
    'View Products' | 'Manage Products' |
    'View Customers' | 'Manage Customers'
    ;

export type PermissionValues =
    'Users.View' | 'Users.Manage' |
    'Roles.View' | 'Roles.Manage' | 'Roles.Assign' |
    'Products.View' | 'Products.Manage' |
    'Customers.View' | 'Customers.Manage'
    ;

export interface Permission {
    name: PermissionNames;
    value: PermissionValues;
    groupName: string;
    description: string;
}

export class Permissions {
    public static readonly viewUsers: PermissionValues = 'Users.View';
    public static readonly manageUsers: PermissionValues = 'Users.Manage';

    public static readonly viewRoles: PermissionValues = 'Roles.View';
    public static readonly manageRoles: PermissionValues = 'Roles.Manage';
    public static readonly assignRoles: PermissionValues = 'Roles.Assign';

    public static readonly viewProducts: PermissionValues = 'Products.View';
    public static readonly manageProducts: PermissionValues = 'Products.Manage';

    public static readonly viewCustomers: PermissionValues = 'Customers.View';
    public static readonly manageCustomers: PermissionValues = 'Customers.Manage';
}
