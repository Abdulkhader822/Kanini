import { 
  AppBar, Toolbar, Typography, Button, Box, Avatar, Menu, MenuItem, 
   Divider, ListItemIcon, ListItemText, 
} from "@mui/material";
import { 
   WorkspacePremium, Edit, Lock, Logout, 
  Dashboard as DashboardIcon, 
} from "@mui/icons-material";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { tokenstore } from "../Auth/tokenstore";

export default function Navbar() {
  const navigate = useNavigate();
  const userName = tokenstore.getUserName() || "Guest";
  const userRole = tokenstore.getRole();
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);

  const handleProfileClick = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const handleLogout = () => {
    tokenstore.clear();
    navigate("/login");
  };

  const handleNavigation = (path: string) => {
    navigate(path);
    handleClose();
  };

  return (
    <AppBar 
      position="static" 
      elevation={0}
      sx={{ 
        background: "linear-gradient(135deg, #667eea 0%, #764ba2 100%)",
        backdropFilter: "blur(10px)",
        borderBottom: "1px solid rgba(255,255,255,0.1)",
        boxShadow: "0 8px 32px rgba(0,0,0,0.1)"
      }}
    >
      <Toolbar sx={{ display: "flex", justifyContent: "space-between", py: 1.5, px: 3 }}>
        <Box display="flex" alignItems="center" gap={2}>
          <Avatar 
            sx={{ 
              bgcolor: "rgba(255,255,255,0.15)", 
              width: 48, 
              height: 48,
              backdropFilter: "blur(10px)",
              border: "2px solid rgba(255,255,255,0.2)",
              transition: "all 0.3s ease",
              "&:hover": {
                transform: "scale(1.05)",
                bgcolor: "rgba(255,255,255,0.25)"
              }
            }}
          >
            <DashboardIcon sx={{ fontSize: 28 }} />
          </Avatar>
          <Box>
            <Typography 
              variant="h5" 
              fontWeight="800" 
              color="white" 
              sx={{ 
                letterSpacing: 0.5,
                textShadow: "0 2px 4px rgba(0,0,0,0.3)",
                background: "linear-gradient(45deg, #ffffff, #e3f2fd)",
                backgroundClip: "text",
                WebkitBackgroundClip: "text",
                WebkitTextFillColor: "transparent"
              }}
            >
              SkillBridge
            </Typography>
            <Typography 
              variant="caption" 
              sx={{ 
                color: "rgba(255,255,255,0.8)",
                fontWeight: 500,
                letterSpacing: 1
              }}
            >
              BRIDGE TO SUCCESS
            </Typography>
          </Box>
        </Box>

        <Box display="flex" alignItems="center" gap={3}>
          <Button
            color="inherit"
            onClick={() => navigate("/about")}
            sx={{ 
              fontWeight: 600,
              textTransform: "none",
              color: "white",
              px: 3,
              py: 1,
              borderRadius: 3,
              background: "rgba(255,255,255,0.1)",
              backdropFilter: "blur(10px)",
              border: "1px solid rgba(255,255,255,0.2)",
              transition: "all 0.3s ease",
              "&:hover": { 
                bgcolor: "rgba(255,255,255,0.2)",
                transform: "translateY(-2px)",
                boxShadow: "0 4px 20px rgba(0,0,0,0.2)"
              }
            }}
          >
            About Us
          </Button>
          
          {userRole === "User" && (
            <Button
              color="inherit"
              startIcon={<WorkspacePremium />}
              onClick={() => navigate("/user/certificates")}
              sx={{ 
                fontWeight: 600,
                textTransform: "none",
                color: "white",
                px: 3,
                py: 1,
                borderRadius: 3,
                background: "rgba(255,255,255,0.1)",
                backdropFilter: "blur(10px)",
                border: "1px solid rgba(255,255,255,0.2)",
                transition: "all 0.3s ease",
                "&:hover": { 
                  bgcolor: "rgba(255,255,255,0.2)",
                  transform: "translateY(-2px)",
                  boxShadow: "0 4px 20px rgba(0,0,0,0.2)"
                }
              }}
            >
              Certificates
            </Button>
          )}

          <Box display="flex" alignItems="center" gap={2}>
            
            <Box 
              display="flex" 
              alignItems="center" 
              gap={1}
              onClick={handleProfileClick}
              sx={{
                cursor: "pointer",
                px: 2,
                py: 1,
                borderRadius: 3,
                transition: "all 0.3s ease",
                "&:hover": {
                  bgcolor: "rgba(255,255,255,0.1)",
                  transform: "translateY(-1px)"
                }
              }}
            >
              <Avatar
                sx={{ 
                  width: 36, 
                  height: 36,
                  bgcolor: "rgba(255,255,255,0.2)",
                  border: "2px solid rgba(255,255,255,0.3)",
                  fontSize: 16,
                  fontWeight: 700
                }}
              >
                {userName.charAt(0).toUpperCase()}
              </Avatar>
              <Box>
                <Typography 
                  variant="subtitle2" 
                  color="white" 
                  sx={{ 
                    fontWeight: 600,
                    lineHeight: 1.2
                  }}
                >
                  {userName}
                </Typography>
                <Typography 
                  variant="caption" 
                  sx={{ 
                    color: "rgba(255,255,255,0.7)",
                    fontSize: "0.7rem"
                  }}
                >
                  Online
                </Typography>
              </Box>
            </Box>
          </Box>

          <Menu
            anchorEl={anchorEl}
            open={Boolean(anchorEl)}
            onClose={handleClose}
            transformOrigin={{ horizontal: 'right', vertical: 'top' }}
            anchorOrigin={{ horizontal: 'right', vertical: 'bottom' }}
            PaperProps={{
              sx: {
                mt: 1,
                minWidth: 220,
                borderRadius: 3,
                boxShadow: "0 8px 40px rgba(0,0,0,0.15)",
                background: "linear-gradient(135deg, #ffffff 0%, #f8fafc 100%)",
                backdropFilter: "blur(20px)",
                border: "1px solid rgba(255,255,255,0.2)",
                overflow: "hidden"
              }
            }}
          >
            <Box sx={{ p: 2, borderBottom: "1px solid rgba(0,0,0,0.05)" }}>
              <Typography variant="subtitle2" fontWeight={600} color="text.primary">
                {userName}
              </Typography>
              <Typography variant="caption" color="text.secondary">
                Welcome to SkillBridge
              </Typography>
            </Box>
            
            <MenuItem 
              onClick={() => handleNavigation("/profile/edit")}
              sx={{
                py: 1.5,
                transition: "all 0.2s ease",
                "&:hover": {
                  bgcolor: "rgba(102, 126, 234, 0.08)",
                  transform: "translateX(4px)"
                }
              }}
            >
              <ListItemIcon>
                <Edit fontSize="small" sx={{ color: "#667eea" }} />
              </ListItemIcon>
              <ListItemText 
                primary="Edit Profile" 
                primaryTypographyProps={{ fontWeight: 500 }}
              />
            </MenuItem>
            
            <MenuItem 
              onClick={() => handleNavigation("/profile/change-password")}
              sx={{
                py: 1.5,
                transition: "all 0.2s ease",
                "&:hover": {
                  bgcolor: "rgba(102, 126, 234, 0.08)",
                  transform: "translateX(4px)"
                }
              }}
            >
              <ListItemIcon>
                <Lock fontSize="small" sx={{ color: "#667eea" }} />
              </ListItemIcon>
              <ListItemText 
                primary="Change Password" 
                primaryTypographyProps={{ fontWeight: 500 }}
              />
            </MenuItem>
            
            <Divider sx={{ my: 1 }} />
            
            <MenuItem 
              onClick={handleLogout} 
              sx={{ 
                py: 1.5,
                color: "error.main",
                transition: "all 0.2s ease",
                "&:hover": {
                  bgcolor: "rgba(244, 67, 54, 0.08)",
                  transform: "translateX(4px)"
                }
              }}
            >
              <ListItemIcon>
                <Logout fontSize="small" color="error" />
              </ListItemIcon>
              <ListItemText 
                primary="Logout" 
                primaryTypographyProps={{ fontWeight: 500 }}
              />
            </MenuItem>
          </Menu>
        </Box>
      </Toolbar>
    </AppBar>
  );
}