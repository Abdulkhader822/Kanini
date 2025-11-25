export interface LoginRequest {
  email: string;
  password: string;
}

export type UserRole = "Admin" | "User";

export interface LoginResponse {
  token: string;
  username: string;
  role: UserRole;
}
