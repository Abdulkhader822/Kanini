import { createRoot } from "react-dom/client";
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import { ThemeProvider } from '@mui/material/styles';
import { CssBaseline } from '@mui/material';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import { theme } from './theme/theme';
import ProtectedRoute from "./Auth/ProtectedRoute";
import Login from "./Pages/Login";
import Register from "./Pages/Register";
import AdminDashboard from "./Pages/Admin/AdminDashboard";
import UserDashboard from "./Pages/UserDashboard";
import UserTestLevels from "./Pages/User/UserTestLevels";
import TakeTest from "./Pages/User/TakeTest";
import ResultPage from "./Pages/User/ResultPage";
import CertificatePage from "./Pages/User/CertificatePage";
import EditProfile from "./Pages/User/EditProfile";
import ChangePassword from "./Pages/User/ChangePassword";
import About from "./Pages/About";
import "./index.css";

//  Router Configuration
const router = createBrowserRouter([
  //  Root redirect
  { path: "/", element: <Login /> },
  
  //  Public Routes
  { path: "/login", element: <Login /> },
  { path: "/register", element: <Register /> },
  { path: "/about", element: <About /> },

  //  Admin Protected Routes
  {
    element: <ProtectedRoute roles={["Admin"]} />,
    children: [
      { path: "/admin/dashboard", element: <AdminDashboard /> },
    ],
  },

  //  User Protected Routes
  {
    element: <ProtectedRoute roles={["User"]} />,
    children: [
      { path: "/user/dashboard", element: <UserDashboard /> },
      { path: "/user/test-levels/:testId", element: <UserTestLevels /> },
      { path: "/user/take-test/:testLevelId", element: <TakeTest /> },
      { path: "/user/result/:resultId", element: <ResultPage /> },
      { path: "/user/certificate", element: <CertificatePage /> },
      { path: "/user/certificates", element: <CertificatePage /> },
      { path: "/profile/edit", element: <EditProfile /> },
      { path: "/profile/change-password", element: <ChangePassword /> },
    ],
  },

  //  Default redirect
  { path: "*", element: <Login /> },
]);

//  Mount React App
createRoot(document.getElementById("root")!).render(
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <RouterProvider router={router} />
      <ToastContainer
        position="top-right"
        autoClose={4000}
        hideProgressBar={false}
        newestOnTop
        closeOnClick
        rtl={false}
        pauseOnFocusLoss
        draggable
        pauseOnHover
        theme="light"
      />
    </ThemeProvider>
);
