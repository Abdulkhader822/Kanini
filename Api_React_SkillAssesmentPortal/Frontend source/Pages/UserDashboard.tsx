import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  Box, Typography, Card, CardContent, Button, FormControl, InputLabel, Select, MenuItem, Chip, Container, Skeleton
} from "@mui/material";
import { toast } from 'react-toastify';
import Layout from "../Layout/Layout";
import { http } from "../Api/http";
import { tokenstore } from "../Auth/tokenstore";
import { jwtDecode } from "jwt-decode";

interface Category {
  categoryId: number;
  categoryName: string;
}

interface Test {
  testId: number;
  testName: string;
  totalQuestions: number;
  durationMins: number;
  categoryName: string;
}

interface Certificate {
  certificateId: number;
  testId: number;
  testName: string;
  issueDate: string;
  certificateUrl: string;
}

const motivationalQuotes = [
  "Every expert was once a beginner.",
  "Your skills define your future.",
  "Challenge yourself. Grow stronger.",
  "Skill grows with challenge, not comfort."
];

export default function UserDashboard() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [allTests, setAllTests] = useState<Test[]>([]);
  const [tests, setTests] = useState<Test[]>([]);
  const [certificates, setCertificates] = useState<Certificate[]>([]);
  const [selectedCategory, setSelectedCategory] = useState<number | "">("");
  const [loading, setLoading] = useState(true);
  const [quote] = useState(motivationalQuotes[Math.floor(Math.random() * motivationalQuotes.length)]);
  const navigate = useNavigate();

  useEffect(() => {
    loadDashboardData();
  }, []);

  useEffect(() => {
    if (selectedCategory !== "") {
      const selectedCategoryName = categories.find(c => c.categoryId === selectedCategory)?.categoryName;
      if (selectedCategoryName) {
        const filteredTests = allTests.filter(test => test.categoryName === selectedCategoryName);
        setTests(filteredTests);
      }
    } else {
      setTests(allTests);
    }
  }, [selectedCategory, allTests, categories]);

  const loadDashboardData = async () => {
    try {
      const { data } = await http.get("/UserDashboard");
      setCategories(data.categories || []);
      setAllTests(data.tests || []);
      setTests(data.tests || []);
      setCertificates(data.certificates || []);
    } catch (err) {
      console.error("Failed to load dashboard data:", err);
    } finally {
      setLoading(false);
    }
  };

  const getUserId = (): number => {
    const token = tokenstore.get();
    if (!token) return 0;
    try {
      const decoded: any = jwtDecode(token);
      return parseInt(decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] || "0");
    } catch {
      return 0;
    }
  };

  const handleViewLevels = (testId: number) => {
    navigate(`/user/test-levels/${testId}`);
  };

  const downloadCertificate = async (testId: number) => {
    const userId = getUserId();
    try {
      const response = await http.get(`/Result/certificate/download/${userId}/${testId}`, {
        responseType: 'blob'
      });
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement('a');
      link.href = url;
      const testName = tests.find(t => t.testId === testId)?.testName || 'Certificate';
      link.setAttribute('download', `${testName.replace(/\s+/g, '_')}_Certificate.pdf`);
      document.body.appendChild(link);
      link.click();
      link.remove();
      window.URL.revokeObjectURL(url);
    } catch (error: any) {
      console.error('Certificate download failed:', error);
      toast.error('Failed to download certificate. Please try again.');
    }
  };

  return (
    <Layout>
      <Box sx={{ background: "linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%)", minHeight: "100vh" }}>
        <Container maxWidth={false} sx={{ px: 4, py: 4, maxWidth: "1400px", mx: "auto" }}>
        {/* Personalized Welcome Banner */}
        <Card elevation={2} sx={{ 
          mb: 4, 
          background: "linear-gradient(135deg, #667eea 0%, #764ba2 100%)", 
          color: "white",
          borderRadius: 3,
          boxShadow: "0 8px 32px rgba(102, 126, 234, 0.2)"
        }}>
          <CardContent sx={{ py: 3 }}>
            <Typography variant="h4" fontWeight="600" gutterBottom>
              Welcome Back, {localStorage.getItem('userName') || 'User'}! 🚀
            </Typography>
            <Typography variant="h6" sx={{ opacity: 0.9, fontWeight: 300 }}>
              "{quote}"
            </Typography>
          </CardContent>
        </Card>

        {/* Category Selection */}
        <Card elevation={1} sx={{ mb: 3, p: 3, border: "1px solid #e0e0e0", borderRadius: 3, bgcolor: "white" }}>
          <Typography variant="h6" fontWeight="600" gutterBottom sx={{ color: "#667eea", mb: 2 }}>
            📚 Filter by Category
          </Typography>
          <FormControl fullWidth sx={{ maxWidth: 400 }}>
            <InputLabel>Choose Category</InputLabel>
            <Select
              value={selectedCategory}
              onChange={(e) => {
                const value = e.target.value as string | number;
                setSelectedCategory(value === "" ? "" : Number(value));
              }}
              label="Choose Category"
              sx={{ borderRadius: 2 }}
            >
              <MenuItem value="">All Categories</MenuItem>
              {categories.map((category) => (
                <MenuItem key={category.categoryId} value={category.categoryId}>
                  {category.categoryName}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        </Card>



        {/* Tests Grid */}
        {!loading && tests.length > 0 && (
          <Card elevation={1} sx={{ p: 3, border: "1px solid #e0e0e0", borderRadius: 2 }}>
            <Typography variant="h6" fontWeight="600" gutterBottom sx={{ color: "#00796B", mb: 3 }}>
              🧩 Available Courses
            </Typography>
            <Box sx={{ display: "grid", gridTemplateColumns: "repeat(auto-fit, minmax(300px, 1fr))", gap: 3 }}>
              {tests.map((test) => {
                const hasCertificate = certificates.some(cert => cert.testId === test.testId);
                return (
                  <Card key={test.testId} elevation={1} sx={{ 
                    height: "100%", 
                    display: "flex", 
                    flexDirection: "column",
                    border: "1px solid #e0e0e0",
                    borderRadius: 2,
                    transition: "all 0.2s ease",
                    "&:hover": { transform: "translateY(-2px)", boxShadow: 2 }
                  }}>
                    <CardContent sx={{ flexGrow: 1, p: 3 }}>
                      <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start", mb: 2 }}>
                        <Typography variant="h6" fontWeight="600" sx={{ color: "#00796B" }}>
                          {test.testName}
                        </Typography>
                        {hasCertificate && <Chip label="✓ Completed" color="success" size="small" sx={{ fontWeight: 600 }} />}
                      </Box>
                      <Box sx={{ display: "flex", flexDirection: "column", gap: 1 }}>
                        <Typography variant="body2" color="textSecondary" sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                          📝 <strong>{test.totalQuestions}</strong> Questions
                        </Typography>
                        <Typography variant="body2" color="textSecondary" sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                          ⏱️ <strong>{test.durationMins}</strong> Minutes
                        </Typography>
                      </Box>
                    </CardContent>
                    <Box sx={{ p: 3, pt: 0 }}>
                      <Button
                        variant={hasCertificate ? "outlined" : "contained"}
                        fullWidth
                        onClick={() => handleViewLevels(test.testId)}
                        sx={{
                          py: 1.5,
                          fontWeight: 600,
                          borderRadius: 2,
                          ...(hasCertificate ? {
                            borderColor: "#00796B",
                            color: "#00796B"
                          } : {
                            bgcolor: "#00796B",
                            "&:hover": { bgcolor: "#FFB300" }
                          })
                        }}
                      >
                        {hasCertificate ? "View Progress" : "Start Test"}
                      </Button>
                    </Box>
                  </Card>
                );
              })}
            </Box>
          </Card>
        )}

        {loading && (
          <Card elevation={1} sx={{ p: 3, border: "1px solid #e0e0e0", borderRadius: 2 }}>
            <Typography variant="h6" fontWeight="600" gutterBottom sx={{ color: "#00796B", mb: 3 }}>
              🧩 Available Courses
            </Typography>
            <Box sx={{ display: "grid", gridTemplateColumns: "repeat(auto-fit, minmax(300px, 1fr))", gap: 3 }}>
              {[1, 2, 3].map((i) => (
                <Card key={i} elevation={1} sx={{ border: "1px solid #e0e0e0", borderRadius: 2 }}>
                  <CardContent sx={{ p: 3 }}>
                    <Skeleton variant="text" width="80%" height={32} sx={{ mb: 2 }} />
                    <Skeleton variant="text" width="60%" height={20} sx={{ mb: 1 }} />
                    <Skeleton variant="text" width="50%" height={20} sx={{ mb: 3 }} />
                    <Skeleton variant="rectangular" width="100%" height={40} sx={{ borderRadius: 2 }} />
                  </CardContent>
                </Card>
              ))}
            </Box>
          </Card>
        )}

        {!loading && tests.length === 0 && (
          <Card elevation={1} sx={{ p: 4, textAlign: "center", border: "1px solid #e0e0e0", borderRadius: 2 }}>
            <Typography variant="h6" color="textSecondary" gutterBottom>
              {selectedCategory !== "" ? "No tests available for this category." : "No tests available."}
            </Typography>
            <Typography variant="body2" color="textSecondary">
              Please check back later or contact your administrator.
            </Typography>
          </Card>
        )}
        </Container>
      </Box>
    </Layout>
  );
}