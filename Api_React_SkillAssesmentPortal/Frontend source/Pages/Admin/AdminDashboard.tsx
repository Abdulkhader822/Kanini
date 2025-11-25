import { useState,  } from "react";
import { Box, Button, Stack, Typography, Paper, Container,  Chip } from "@mui/material";
import { AdminPanelSettings as AdminIcon } from "@mui/icons-material";
import Navbar from "../../Layout/Navbar";
import Footer from "../../Layout/Footer";
import UserManagement from "./UserManagement";
import CategoryManager from "./CategoryManager";
import TestManager from "./TestManager";
import TestLevelManager from "./TestLevelManager";
import QuestionManager from "./QuestionManager";

const tabConfig = [
  { key: "users", label: "Users", icon: "👥" },
  { key: "category", label: "Categories", icon: "📚" },
  { key: "test", label: "Courses", icon: "📝" },
  { key: "level", label: "Levels", icon: "📊" },
  { key: "question", label: "Questions", icon: "❓" }
];

export default function AdminDashboard() {
  const [tab, setTab] = useState("users");

  // No toast clearing - let toasts show naturally

  const renderContent = () => {
    switch (tab) {
      case "users":
        return <UserManagement />;
      case "category":
        return <CategoryManager />;
      case "test":
        return <TestManager />;
      case "level":
        return <TestLevelManager />;
      case "question":
        return <QuestionManager />;
      default:
        return null;
    }
  };

  const activeTab = tabConfig.find(t => t.key === tab);

  return (
    <Box sx={{ 
      background: "linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%)", 
      minHeight: "100vh" 
    }}>
      <Navbar />
      <Container maxWidth={false} sx={{ px: 4, py: 4, maxWidth: "1400px", mx: "auto" }}>
        {/* Professional Header */}
        <Paper elevation={2} sx={{ 
          mb: 4, 
          background: "linear-gradient(135deg, #667eea 0%, #764ba2 100%)", 
          color: "white",
          borderRadius: 3,
          boxShadow: "0 8px 32px rgba(102, 126, 234, 0.2)"
        }}>
          <Box sx={{ p: 4, textAlign: "center" }}>
            <AdminIcon sx={{ fontSize: 48, mb: 2, opacity: 0.9 }} />
            <Typography variant="h3" fontWeight="700" gutterBottom sx={{ letterSpacing: 1 }}>
              Admin Dashboard
            </Typography>
            <Typography variant="h6" sx={{ opacity: 0.9, fontWeight: 300 }}>
              Skill Assessment Portal Management
            </Typography>
          </Box>
        </Paper>

        {/* Navigation Tabs */}
        <Paper elevation={1} sx={{ mb: 3, p: 2, borderRadius: 3, bgcolor: "white" }}>
          <Stack
            direction="row"
            spacing={1}
            justifyContent="center"
            alignItems="center"
            sx={{ flexWrap: "wrap", gap: 1 }}
          >
            {tabConfig.map((tabItem) => (
              <Button
                key={tabItem.key}
                variant={tab === tabItem.key ? "contained" : "outlined"}
                onClick={() => setTab(tabItem.key)}
                startIcon={<span>{tabItem.icon}</span>}
                sx={{
                  fontWeight: 600,
                  px: 3,
                  py: 1.5,
                  borderRadius: 2,
                  textTransform: "none",
                  minWidth: 120,
                  ...(tab === tabItem.key && {
                    background: "linear-gradient(135deg, #667eea, #764ba2)",
                    "&:hover": { background: "linear-gradient(135deg, #5a67d8, #6b46c1)" }
                  })
                }}
              >
                {tabItem.label}
              </Button>
            ))}
          </Stack>
        </Paper>

        {/* Active Section Header */}
        <Box sx={{ mb: 3, display: "flex", alignItems: "center", gap: 2 }}>
          <Chip 
            icon={<span>{activeTab?.icon}</span>}
            label={`Managing ${activeTab?.label}`}
            variant="outlined"
            sx={{ 
              fontSize: "1rem", 
              fontWeight: 600,
              py: 2,
              px: 1,
              borderColor: "#667eea",
              color: "#667eea"
            }}
          />
        </Box>

        {/* Content Area */}
        <Paper
          elevation={1}
          sx={{
            p: 4,
            borderRadius: 3,
            backgroundColor: "#fff",
            border: "1px solid #e0e0e0"
          }}
        >
          {renderContent()}
        </Paper>
      </Container>
      <Footer />
    </Box>
  );
}
