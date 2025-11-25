import { Navigate, Outlet } from "react-router-dom";
import { tokenstore } from "./tokenstore";

interface ProtectedRouteProps {
  roles?: string[];
  redirectTo?: string;
}

export default function ProtectedRoute({
  roles,
  redirectTo = "/login",
}: ProtectedRouteProps) {
  const token = tokenstore.get();
  const role = tokenstore.getRole();

  if (!token) {
    return <Navigate to={redirectTo} replace />;
  }

  if (roles && roles.length > 0 && !roles.includes(role || "")) {
    return <Navigate to={redirectTo} replace />;
  }

  return <Outlet />;
}
