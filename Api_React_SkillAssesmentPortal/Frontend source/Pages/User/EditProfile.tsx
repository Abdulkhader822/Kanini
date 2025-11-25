import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import {
  Box, Paper, TextField, Button, Typography, Alert, Container
} from "@mui/material";
import { ArrowBack } from "@mui/icons-material";
import { toast } from 'react-toastify';
import Navbar from "../../Layout/Navbar";
import { http } from "../../Api/http";
import { tokenstore } from "../../Auth/tokenstore";

interface UserProfile {
  userId: number;
  name: string;
  email: string;
  role: string;
}

export default function EditProfile() {
  const [profile, setProfile] = useState<UserProfile>({
    userId: 0,
    name: "",
    email: "",
    role: ""
  });
  const [loading, setLoading] = useState(false);
  const [fieldErrors, setFieldErrors] = useState({
    name: "",
    email: ""
  });
  const navigate = useNavigate();

  useEffect(() => {
    loadProfile();
  }, []);

  const loadProfile = async () => {
    try {
      const userId = tokenstore.getUserId();
      if (!userId) {
        navigate("/login");
        return;
      }
      
      const { data } = await http.get(`/User/${userId}`);
      setProfile(data);
    } catch (err: any) {
      // Error handled by HTTP interceptor
    }
  };

  const validateName = (name: string) => {
    if (!name.trim()) return "Name is required";
    if (name.trim().length < 2) return "Name must be at least 2 characters";
    return "";
  };

  const validateEmail = (email: string) => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!email) return "Email is required";
    if (!emailRegex.test(email)) return "Please enter a valid email address";
    return "";
  };

  const handleFieldChange = (field: string, value: string) => {
    setProfile({ ...profile, [field]: value });

    let fieldError = "";
    if (field === "name") fieldError = validateName(value);
    if (field === "email") fieldError = validateEmail(value);

    setFieldErrors({ ...fieldErrors, [field]: fieldError });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const nameErr = validateName(profile.name);
    const emailErr = validateEmail(profile.email);

    if (nameErr || emailErr) {
      setFieldErrors({ name: nameErr, email: emailErr });
      return;
    }

    setLoading(true);
    try {
      await http.put(`/User/${profile.userId}`, {
        name: profile.name,
        email: profile.email
      });
      
      tokenstore.setUserName(profile.name);
      toast.success("Profile updated successfully!");
      
      setTimeout(() => {
        const role = tokenstore.getRole();
        navigate(role === "Admin" ? "/admin/dashboard" : "/user/dashboard");
      }, 2000);
    } catch (err: any) {
      // All errors handled by HTTP interceptor
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box sx={{ 
      background: "linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%)", 
      minHeight: "100vh" 
    }}>
      <Navbar />
      <Container maxWidth="md" sx={{ py: 4 }}>
        <Paper elevation={1} sx={{ p: 4, borderRadius: 3, bgcolor: "white" }}>
          <Box sx={{ display: "flex", alignItems: "center", mb: 3 }}>
            <Button
              startIcon={<ArrowBack />}
              onClick={() => navigate(-1)}
              sx={{ mr: 2, color: "#667eea" }}
            >
              Back
            </Button>
            <Typography variant="h4" fontWeight="700" sx={{ color: "#667eea" }}>
              Edit Profile
            </Typography>
          </Box>



          <Box component="form" onSubmit={handleSubmit} sx={{ maxWidth: 500 }}>
            <TextField
              label="Full Name"
              value={profile.name}
              onChange={(e) => handleFieldChange("name", e.target.value)}
              error={!!fieldErrors.name}
              helperText={fieldErrors.name}
              fullWidth
              margin="normal"
              variant="outlined"
            />
            
            <TextField
              label="Email Address"
              type="email"
              value={profile.email}
              onChange={(e) => handleFieldChange("email", e.target.value)}
              error={!!fieldErrors.email}
              helperText={fieldErrors.email}
              fullWidth
              margin="normal"
              variant="outlined"
            />
            
            <TextField
              label="Role"
              value={profile.role}
              fullWidth
              margin="normal"
              variant="outlined"
              disabled
              helperText="Role cannot be changed"
            />

            <Box sx={{ mt: 3, display: "flex", gap: 2 }}>
              <Button
                type="submit"
                variant="contained"
                disabled={loading || !!fieldErrors.name || !!fieldErrors.email}
                sx={{
                  px: 4,
                  py: 1.5,
                  background: "linear-gradient(135deg, #667eea, #764ba2)",
                  "&:hover": { background: "linear-gradient(135deg, #5a67d8, #6b46c1)" }
                }}
              >
                {loading ? "Updating..." : "Update Profile"}
              </Button>
              
              <Button
                variant="outlined"
                onClick={() => navigate(-1)}
                sx={{
                  px: 4,
                  py: 1.5,
                  borderColor: "#667eea",
                  color: "#667eea",
                  "&:hover": { borderColor: "#5a67d8", color: "#5a67d8" }
                }}
              >
                Cancel
              </Button>
            </Box>
          </Box>
        </Paper>
      </Container>
    </Box>
  );
}