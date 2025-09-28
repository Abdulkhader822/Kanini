export interface Role {
  roleId: number;           // ✅ int
  roleName: string;
}

export interface User {
  userId: number;           // ✅ int
  userName: string;
  email: string;
  password?: string;        // used for login/register
  passwordHash?: string;    // from backend
  role: Role;
}
