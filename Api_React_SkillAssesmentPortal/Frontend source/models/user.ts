import type { UserRole } from "./auth";

export interface UserResponse {
  userId: number;
  name: string;
  email: string;
  role: UserRole;
  createdAt: string; // ISO string
}

export interface UserCreate {
  name: string;
  email: string;
  password: string;
  role?: UserRole;   // default handled by backend
}

export interface UserUpdate {
  name?: string;
  email?: string;
  role?: UserRole;
  password?: string;
}
