import { useState, type FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import {
  Box, Paper, TextField, Button, Typography, Alert, Link, MenuItem
} from "@mui/material";
import { toast } from 'react-toastify';
import { http } from "../Api/http";

export default function Register() {
  const [form, setForm] = useState({
    name: "",
    email: "",
    password: "",
    role: "User",
  });
  const [loading, setLoading] = useState(false);
  const [fieldErrors, setFieldErrors] = useState({
    name: "",
    email: "",
    password: ""
  });
  const navigate = useNavigate();

  const validateName = (name: string) => {
    if (!name.trim()) return "Full name is required";
    if (name.trim().length < 2) return "Name must be at least 2 characters";
    return "";
  };

  const validateEmail = (email: string) => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!email) return "Email is required";
    if (!emailRegex.test(email)) return "Please enter a valid email address";
    return "";
  };

  const validatePassword = (password: string) => {
    if (!password) return "Password is required";
    if (password.length < 6) return "Password must be at least 6 characters";
    if (!/(?=.*[a-z])(?=.*[A-Z])/.test(password)) return "Password must contain both uppercase and lowercase letters";
    return "";
  };

  const handleFieldChange = (field: string, value: string) => {
    setForm({ ...form, [field]: value });

    let fieldError = "";
    switch (field) {
      case "name":
        fieldError = validateName(value);
        break;
      case "email":
        fieldError = validateEmail(value);
        break;
      case "password":
        fieldError = validatePassword(value);
        break;
    }

    setFieldErrors({ ...fieldErrors, [field]: fieldError });
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();

    const nameErr = validateName(form.name);
    const emailErr = validateEmail(form.email);
    const passwordErr = validatePassword(form.password);

    if (nameErr || emailErr || passwordErr) {
      setFieldErrors({
        name: nameErr,
        email: emailErr,
        password: passwordErr
      });
      return;
    }

    setLoading(true);
    try {
      const res = await http.post("/User", form);
      if (res.status === 200) {
        toast.success("Account created successfully! Redirecting to login...");
        setTimeout(() => navigate("/login"), 2000);
      }
    } catch (err: any) {
      // All errors handled by HTTP interceptor
    } finally {
      setLoading(false);
    }
  };

  const hasErrors = Object.values(fieldErrors).some(error => error !== "");
  const hasEmptyFields = !form.name.trim() || !form.email.trim() || !form.password.trim();

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
            Join SkillBridge
          </Typography>
          <Typography variant="subtitle1" sx={{ color: "#64748b", fontWeight: 500 }}>
            Bridge Your Skills to Success
          </Typography>
        </Box>



        <Box component="form" onSubmit={handleSubmit} sx={{ display: "flex", flexDirection: "column", gap: 2.5 }}>
          <TextField
            label="Full Name"
            value={form.name}
            onChange={(e) => handleFieldChange("name", e.target.value)}
            error={!!fieldErrors.name}
            helperText={fieldErrors.name}
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
            type="email"
            label="Email Address"
            value={form.email}
            onChange={(e) => handleFieldChange("email", e.target.value)}
            error={!!fieldErrors.email}
            helperText={fieldErrors.email}
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
            value={form.password}
            onChange={(e) => handleFieldChange("password", e.target.value)}
            error={!!fieldErrors.password}
            helperText={fieldErrors.password}
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
            select
            label="Role"
            value={form.role}
            onChange={(e) => setForm({ ...form, role: e.target.value })}
            fullWidth
            variant="outlined"
            disabled
            sx={{
              "& .MuiOutlinedInput-root": {
                backgroundColor: "rgba(255, 255, 255, 0.8)"
              }
            }}
          >
            <MenuItem value="User">User</MenuItem>
          </TextField>

          <Button
            type="submit"
            variant="contained"
            size="large"
            disabled={loading || hasErrors || hasEmptyFields}
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
            {loading ? "Registering..." : "Register"}
          </Button>
        </Box>

        <Typography align="center" sx={{ mt: 4, color: "#64748b" }}>
          Already have an account?{" "}
          <Link
            component="button"
            type="button"
            onClick={() => navigate("/login")}
            sx={{ 
              fontWeight: 600, 
              color: "#667eea",
              textDecoration: "none",
              "&:hover": { color: "#5a67d8" }
            }}
          >
            Login here
          </Link>
        </Typography>
      </Paper>
    </Box>
  );
}