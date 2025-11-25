import { useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  Box, Paper, TextField, Button, Typography, Alert, Container,
  InputAdornment, IconButton
} from "@mui/material";
import { ArrowBack, Visibility, VisibilityOff } from "@mui/icons-material";
import { toast } from 'react-toastify';
import Navbar from "../../Layout/Navbar";
import { http } from "../../Api/http";
import { tokenstore } from "../../Auth/tokenstore";

export default function ChangePassword() {
  const [form, setForm] = useState({
    currentPassword: "",
    newPassword: "",
    confirmPassword: ""
  });
  const [showPasswords, setShowPasswords] = useState({
    current: false,
    new: false,
    confirm: false
  });
  const [loading, setLoading] = useState(false);
  const [fieldErrors, setFieldErrors] = useState({
    currentPassword: "",
    newPassword: "",
    confirmPassword: ""
  });
  const navigate = useNavigate();

  const validateCurrentPassword = (password: string) => {
    if (!password) return "Current password is required";
    return "";
  };

  const validateNewPassword = (password: string) => {
    if (!password) return "New password is required";
    if (password.length < 6) return "Password must be at least 6 characters";
    if (!/(?=.*[a-z])(?=.*[A-Z])/.test(password)) return "Password must contain both uppercase and lowercase letters";
    return "";
  };

  const validateConfirmPassword = (password: string, newPassword: string) => {
    if (!password) return "Please confirm your new password";
    if (password !== newPassword) return "Passwords do not match";
    return "";
  };

  const handleFieldChange = (field: string, value: string) => {
    setForm({ ...form, [field]: value });

    let fieldError = "";
    switch (field) {
      case "currentPassword":
        fieldError = validateCurrentPassword(value);
        break;
      case "newPassword":
        fieldError = validateNewPassword(value);
        // Also revalidate confirm password if it exists
        if (form.confirmPassword) {
          const confirmError = validateConfirmPassword(form.confirmPassword, value);
          setFieldErrors(prev => ({ ...prev, confirmPassword: confirmError }));
        }
        break;
      case "confirmPassword":
        fieldError = validateConfirmPassword(value, form.newPassword);
        break;
    }

    setFieldErrors({ ...fieldErrors, [field]: fieldError });
  };

  const togglePasswordVisibility = (field: string) => {
    setShowPasswords({ ...showPasswords, [field]: !showPasswords[field as keyof typeof showPasswords] });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const currentErr = validateCurrentPassword(form.currentPassword);
    const newErr = validateNewPassword(form.newPassword);
    const confirmErr = validateConfirmPassword(form.confirmPassword, form.newPassword);

    if (currentErr || newErr || confirmErr) {
      setFieldErrors({
        currentPassword: currentErr,
        newPassword: newErr,
        confirmPassword: confirmErr
      });
      return;
    }

    setLoading(true);
    try {
      const userId = tokenstore.getUserId();
      await http.put(`/User/${userId}/change-password`, {
        currentPassword: form.currentPassword,
        newPassword: form.newPassword
      });
      
      toast.success("Password changed successfully! You will be redirected to login.");
      
      setTimeout(() => {
        tokenstore.clear();
        navigate("/login");
      }, 3000);
    } catch (err: any) {
      // All errors handled by HTTP interceptor
    } finally {
      setLoading(false);
    }
  };

  const hasErrors = Object.values(fieldErrors).some(error => error !== "");
  const hasEmptyFields = !form.currentPassword || !form.newPassword || !form.confirmPassword;

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
              Change Password
            </Typography>
          </Box>



          <Box component="form" onSubmit={handleSubmit} sx={{ maxWidth: 500 }}>
            <TextField
              label="Current Password"
              type={showPasswords.current ? "text" : "password"}
              value={form.currentPassword}
              onChange={(e) => handleFieldChange("currentPassword", e.target.value)}
              error={!!fieldErrors.currentPassword}
              helperText={fieldErrors.currentPassword}
              fullWidth
              margin="normal"
              variant="outlined"
              InputProps={{
                endAdornment: (
                  <InputAdornment position="end">
                    <IconButton onClick={() => togglePasswordVisibility("current")}>
                      {showPasswords.current ? <VisibilityOff /> : <Visibility />}
                    </IconButton>
                  </InputAdornment>
                )
              }}
            />
            
            <TextField
              label="New Password"
              type={showPasswords.new ? "text" : "password"}
              value={form.newPassword}
              onChange={(e) => handleFieldChange("newPassword", e.target.value)}
              error={!!fieldErrors.newPassword}
              helperText={fieldErrors.newPassword}
              fullWidth
              margin="normal"
              variant="outlined"
              InputProps={{
                endAdornment: (
                  <InputAdornment position="end">
                    <IconButton onClick={() => togglePasswordVisibility("new")}>
                      {showPasswords.new ? <VisibilityOff /> : <Visibility />}
                    </IconButton>
                  </InputAdornment>
                )
              }}
            />
            
            <TextField
              label="Confirm New Password"
              type={showPasswords.confirm ? "text" : "password"}
              value={form.confirmPassword}
              onChange={(e) => handleFieldChange("confirmPassword", e.target.value)}
              error={!!fieldErrors.confirmPassword}
              helperText={fieldErrors.confirmPassword}
              fullWidth
              margin="normal"
              variant="outlined"
              InputProps={{
                endAdornment: (
                  <InputAdornment position="end">
                    <IconButton onClick={() => togglePasswordVisibility("confirm")}>
                      {showPasswords.confirm ? <VisibilityOff /> : <Visibility />}
                    </IconButton>
                  </InputAdornment>
                )
              }}
            />

            <Box sx={{ mt: 3, display: "flex", gap: 2 }}>
              <Button
                type="submit"
                variant="contained"
                disabled={loading || hasErrors || hasEmptyFields}
                sx={{
                  px: 4,
                  py: 1.5,
                  background: "linear-gradient(135deg, #667eea, #764ba2)",
                  "&:hover": { background: "linear-gradient(135deg, #5a67d8, #6b46c1)" }
                }}
              >
                {loading ? "Changing..." : "Change Password"}
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