import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  Box, Typography, Card, CardContent, Button, Alert, Avatar, Paper, Container
} from "@mui/material";
import { Download as DownloadIcon, EmojiEvents as TrophyIcon } from "@mui/icons-material";
import Navbar from "../../Layout/Navbar";
import { http } from "../../Api/http";
import { tokenstore } from "../../Auth/tokenstore";

interface Certificate {
  certificateId: number;
  testName: string;
  userName: string;
  completionDate: string;
  certificateUrl?: string;
  testId: number;
}

export default function CertificatePage() {
  const [certificates, setCertificates] = useState<Certificate[]>([]);
  const [loading, setLoading] = useState(true);
  const [downloading, setDownloading] = useState(false);

  const navigate = useNavigate();

  useEffect(() => {
    loadCertificates();
  }, []);

  const handleDownloadCertificate = async (certificateId: number, testName: string) => {
    setDownloading(true);
    try {
      const userId = tokenstore.getUserId();
      if (!userId) {
        alert("User not authenticated");
        return;
      }

      const cert = certificates.find(c => c.certificateId === certificateId);
      if (!cert) {
        alert("Certificate not found");
        return;
      }

      const response = await http.get(`/Result/certificate/download/${userId}/${cert.testId}`, {
        responseType: 'blob'
      });
      
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', `${testName.replace(/\s+/g, '_')}_Certificate.pdf`);
      document.body.appendChild(link);
      link.click();
      link.remove();
      window.URL.revokeObjectURL(url);
    } catch (err: any) {

      alert("Certificate download failed. Please try again.");
    } finally {
      setDownloading(false);
    }
  };

  const loadCertificates = async () => {
    try {
      const userId = tokenstore.getUserId();
      if (userId) {
        const { data } = await http.get<Certificate[]>(`/Certificate/user/${userId}`);

        setCertificates(data);
      }
    } catch (err: any) {

      if (err.response?.status === 404) {
        setCertificates([]);
      }
    } finally {
      setLoading(false);
    }
  };



  if (loading) {
    return (
      <Box sx={{ 
        background: "linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%)", 
        minHeight: "100vh" 
      }}>
        <Navbar />
        <Container maxWidth={false} sx={{ px: 4, py: 4, maxWidth: "1400px", mx: "auto", textAlign: "center" }}>
          <Typography variant="h6">Loading certificates...</Typography>
        </Container>
      </Box>
    );
  }

  return (
    <Box sx={{ 
      background: "linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%)", 
      minHeight: "100vh" 
    }}>
      <Navbar />
      <Container maxWidth={false} sx={{ px: 4, py: 4, maxWidth: "1400px", mx: "auto" }}>
        <Paper elevation={2} sx={{ 
          mb: 4, 
          background: "linear-gradient(135deg, #667eea 0%, #764ba2 100%)", 
          color: "white",
          borderRadius: 3,
          boxShadow: "0 8px 32px rgba(102, 126, 234, 0.2)"
        }}>
          <CardContent sx={{ textAlign: "center", py: 4 }}>
            <Avatar sx={{ 
              bgcolor: "rgba(255,255,255,0.15)", 
              mx: "auto", 
              mb: 2, 
              width: 80, 
              height: 80,
              border: "3px solid rgba(255,255,255,0.3)"
            }}>
              <TrophyIcon sx={{ fontSize: 40, color: "#ffd700" }} />
            </Avatar>
            <Typography variant="h3" fontWeight="700" gutterBottom sx={{ letterSpacing: 1 }}>
              Certificate of Achievement
            </Typography>
            <Typography variant="h6" sx={{ opacity: 0.9, fontWeight: 300 }}>
              Professional Recognition of Excellence
            </Typography>
          </CardContent>
        </Paper>

        {certificates.length > 0 ? (
          <>
            <Typography variant="h5" gutterBottom sx={{ mb: 4, fontWeight: 600, color: "#667eea" }}>
              📜 Your Certificates ({certificates.length})
            </Typography>
            
            <Box sx={{ display: "grid", gridTemplateColumns: "repeat(auto-fit, minmax(400px, 1fr))", gap: 3 }}>
              {certificates.map((certificate) => (
                <Paper key={certificate.certificateId} elevation={1} sx={{ 
                  border: "1px solid #e0e0e0",
                  borderRadius: 3,
                  overflow: "hidden",
                  bgcolor: "white",
                  transition: "all 0.3s ease",
                  "&:hover": { 
                    transform: "translateY(-4px)", 
                    boxShadow: "0 8px 25px rgba(102, 126, 234, 0.15)",
                    borderColor: "#667eea"
                  }
                }}>
                  <Box sx={{ 
                    background: "linear-gradient(90deg, #f8f9fa, #ffffff)",
                    borderBottom: "1px solid #e0e0e0"
                  }}>
                    <CardContent sx={{ pb: 2 }}>
                      <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start" }}>
                        <Box sx={{ flex: 1 }}>
                          <Typography variant="h5" fontWeight="600" gutterBottom sx={{ 
                            color: "#667eea",
                            mb: 2
                          }}>
                            {certificate.testName}
                          </Typography>
                          <Box sx={{ display: "flex", flexDirection: "column", gap: 1 }}>
                            <Typography variant="body1" sx={{ fontWeight: 500 }}>
                              Recipient: <span style={{ color: "#1976d2", fontWeight: 600 }}>{certificate.userName}</span>
                            </Typography>
                            <Typography variant="body1" color="textSecondary">
                              Issued Date: <span style={{ fontWeight: 500 }}>{new Date(certificate.completionDate).toLocaleDateString('en-US', { 
                                year: 'numeric', 
                                month: 'long', 
                                day: 'numeric' 
                              })}</span>
                            </Typography>
                          </Box>
                          <Box sx={{ mt: 3, pt: 2, borderTop: "1px solid #e0e0e0" }}>
                            <Typography variant="body2" color="textSecondary" align="center">
                              Digital Signature
                            </Typography>
                            <Typography variant="body1" fontWeight="600" align="center" sx={{ 
                              mt: 1,
                              fontFamily: "'Brush Script MT', cursive",
                              fontSize: "1.2rem",
                              color: "#667eea"
                            }}>
                              Skill Assessment Portal
                            </Typography>
                          </Box>
                        </Box>
                        

                      </Box>
                    </CardContent>
                  </Box>
                </Paper>
              ))}
            </Box>

            <Paper elevation={0} sx={{ 
              mt: 4, 
              p: 3, 
              bgcolor: "#f8f9fa", 
              border: "1px solid #e8f5e9",
              borderRadius: 3
            }}>
              <Typography variant="h6" sx={{ color: "#2e7d32", fontWeight: 600, mb: 1 }}>
                🎯 Congratulations!
              </Typography>
              <Typography variant="body1" color="textSecondary">
                You've successfully completed all levels and earned your certificates. 
                Keep up the excellent work and continue your learning journey!
              </Typography>
            </Paper>
          </>
        ) : (
          <Card sx={{ textAlign: "center", p: 6, borderRadius: 3, bgcolor: "white" }}>
            <Typography variant="h5" color="textSecondary" gutterBottom>
              📋 No Certificates Yet
            </Typography>
            <Typography variant="body1" color="textSecondary" gutterBottom sx={{ mb: 3 }}>
              Complete all levels of a test to earn your certificate!
            </Typography>
            <Button
              variant="contained"
              onClick={() => navigate("/user/dashboard")}
              sx={{ 
                px: 4,
                py: 1.5,
                background: "linear-gradient(135deg, #667eea, #764ba2)",
                "&:hover": { background: "linear-gradient(135deg, #5a67d8, #6b46c1)" }
              }}
            >
              Start Taking Tests
            </Button>
          </Card>
        )}

        <Box sx={{ textAlign: "center", mt: 4 }}>
          <Button
            variant="outlined"
            onClick={() => navigate("/user/dashboard")}
            size="large"
            sx={{
              px: 4,
              py: 1.5,
              borderColor: "#667eea",
              color: "#667eea",
              "&:hover": { borderColor: "#5a67d8", color: "#5a67d8" }
            }}
          >
            ← Back to Dashboard
          </Button>
        </Box>
      </Container>
    </Box>
  );
}