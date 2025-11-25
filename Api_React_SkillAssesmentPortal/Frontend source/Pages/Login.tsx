import { useState, type FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import {
  Box, Paper, TextField, Button, Typography,  Link
} from "@mui/material";
import { toast } from 'react-toastify';
import { http } from "../Api/http";
import { tokenstore } from "../Auth/tokenstore";

export default function Login() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [emailError, setEmailError] = useState("");
  const [passwordError, setPasswordError] = useState("");
  const navigate = useNavigate();

  const validateEmail = (email: string) => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!email) return "Email is required";
    if (!emailRegex.test(email)) return "Please enter a valid email address";
    return "";
  };

  const validatePassword = (password: string) => {
    if (!password) return "Password is required";
    if (password.length < 6) return "Password must be at least 6 characters";
    return "";
  };

  const handleEmailChange = (value: string) => {
    setEmail(value);
    setEmailError(validateEmail(value));
  };

  const handlePasswordChange = (value: string) => {
    setPassword(value);
    setPasswordError(validatePassword(value));
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    
    // Prevent duplicate submissions
    if (loading) return;
    
    const emailErr = validateEmail(email);
    const passwordErr = validatePassword(password);
    
    if (emailErr || passwordErr) {
      setEmailError(emailErr);
      setPasswordError(passwordErr);
      return;
    }

    setLoading(true);

    try {
      const { data } = await http.post("/Auth/login", { email, password });
      
      // Check if login was successful
      if (data?.token && data?.role) {
        tokenstore.set(data.token);
        tokenstore.setRole(data.role);
        localStorage.setItem("userName", data.username || data.email || "User");
        
        toast.success("Login successful!");
        
        if (data.role === "Admin") {
          navigate("/admin/dashboard");
        } else if (data.role === "User") {
          navigate("/user/dashboard");
        } else {
          toast.error("Unauthorized role.");
        }
      } else {
        toast.error("Login failed. Invalid response from server.");
      }
    } catch (err: any) {

      const errorMessage = err.response?.data?.error || "Login failed. Please try again.";
      toast.error(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box sx={{
      minHeight: "100vh",
      display: "flex",
      alignItems: "center",
      justifyContent: "center",
      background: "linear-gradient(135deg, #667eea 0%, #764ba2 100%)",
      backgroundImage: "url('https://images.unsplash.com/photo-1481627834876-b7833e8f5570?ixlib=rb-4.0.3&auto=format&fit=crop&w=2000&q=80')",
      backgroundSize: "cover",
      backgroundPosition: "center",
      backgroundBlendMode: "overlay",
      p: 2
    }}>
      <Paper elevation={0} sx={{
        p: 6,
        width: "100%",
        maxWidth: 460,
        backgroundColor: "rgba(255, 255, 255, 0.98)",
        backdropFilter: "blur(30px)",
        borderRadius: 5,
        border: "1px solid rgba(255, 255, 255, 0.3)",
        boxShadow: "0 25px 50px rgba(102, 126, 234, 0.25)"
      }}>
        <Box sx={{ textAlign: "center", mb: 4 }}>
          <Typography variant="h3" sx={{ 
            fontWeight: 800, 
            background: "linear-gradient(135deg, #667eea, #764ba2)",
            backgroundClip: "text",
            WebkitBackgroundClip: "text",
            WebkitTextFillColor: "transparent",
            mb: 1,
            letterSpacing: 1
          }}>
            SkillBridge
          </Typography>
          <Typography variant="subtitle1" sx={{ color: "#64748b", fontWeight: 500 }}>
            Bridge Your Skills to Success
          </Typography>
        </Box>



        <Box component="form" onSubmit={handleSubmit} sx={{ display: "flex", flexDirection: "column", gap: 2.5 }}>
          <TextField
            type="email"
            label="Email Address"
            value={email}
            onChange={(e) => handleEmailChange(e.target.value)}
            error={!!emailError}
            helperText={emailError}
            required
            fullWidth
            variant="outlined"
            sx={{
              "& .MuiOutlinedInput-root": {
                backgroundColor: "rgba(255, 255, 255, 0.8)"
              }
            }}
          />
          <TextField
            type="password"
            label="Password"
            value={password}
            onChange={(e) => handlePasswordChange(e.target.value)}
            error={!!passwordError}
            helperText={passwordError}
            required
            fullWidth
            variant="outlined"
            sx={{
              "& .MuiOutlinedInput-root": {
                backgroundColor: "rgba(255, 255, 255, 0.8)"
              }
            }}
          />
          <Button
            type="submit"
            variant="contained"
            size="large"
            disabled={loading || !!emailError || !!passwordError || !email.trim() || !password.trim()}
            sx={{ 
              mt: 2, 
              py: 2,
              background: "linear-gradient(135deg, #667eea, #764ba2)",
              "&:hover": { 
                background: "linear-gradient(135deg, #5a67d8, #6b46c1)",
                transform: "translateY(-2px)",
                boxShadow: "0 8px 25px rgba(102, 126, 234, 0.3)"
              },
              fontWeight: 600,
              borderRadius: 3,
              fontSize: "1.1rem",
              transition: "all 0.3s ease"
            }}
          >
            {loading ? "Logging in..." : "Login"}
          </Button>
        </Box>

        <Typography align="center" sx={{ mt: 4, color: "#64748b" }}>
          Don't have an account?{" "}
          <Link
            component="button"
            type="button"
            onClick={() => navigate("/register")}
            sx={{ 
              fontWeight: 600, 
              color: "#667eea",
              textDecoration: "none",
              "&:hover": { color: "#5a67d8" }
            }}
          >
            Register here
          </Link>
        </Typography>
      </Paper>
    </Box>
  );
}