import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import {
  Box, Typography, Card, CardContent, Button, Chip, Container, Alert, Skeleton
} from "@mui/material";
import { PlayArrow as PlayIcon } from "@mui/icons-material";
import Navbar from "../../Layout/Navbar";
import { http } from "../../Api/http";
import VideoModal from "./VideoModal";

interface TestLevel {
  testLevelId: number;
  testId: number;
  levelName: string;
  passingScore: number;
  videoLink: string;
  durationMins: number;
}

export default function UserTestLevels() {
  const { testId } = useParams<{ testId: string }>();
  const [levels, setLevels] = useState<TestLevel[]>([]);
  const [loading, setLoading] = useState(true);
  const [videoOpen, setVideoOpen] = useState(false);
  const [selectedVideo, setSelectedVideo] = useState("");
  const [completedLevels, setCompletedLevels] = useState<string[]>([]);
  const [hasCertificate, setHasCertificate] = useState(false);
  const navigate = useNavigate();

  // Refresh completed levels - can be called after test completion
  const refreshCompletedLevels = () => {
    if (testId) {
      loadCompletedLevels(Number(testId));
    }
  };

  // Listen for storage events to refresh when returning from test
  useEffect(() => {
    const handleStorageChange = (e: StorageEvent) => {
      if (e.key === 'testCompleted') {
        refreshCompletedLevels();
        localStorage.removeItem('testCompleted');
      }
    };
    
    // Check for testCompleted flag on mount
    if (localStorage.getItem('testCompleted')) {
      setTimeout(() => {
        refreshCompletedLevels();
        localStorage.removeItem('testCompleted');
      }, 500);
    }
    
    window.addEventListener('storage', handleStorageChange);
    return () => window.removeEventListener('storage', handleStorageChange);
  }, [testId]);

  useEffect(() => {
    if (testId) {
      loadTestLevels(Number(testId));
      loadCompletedLevels(Number(testId));
      checkCertificate(Number(testId));
    }
  }, [testId]);

  // Refresh when component becomes visible (user returns from test)
  useEffect(() => {
    const handleVisibilityChange = () => {
      if (!document.hidden && testId) {
        refreshCompletedLevels();
      }
    };
    document.addEventListener('visibilitychange', handleVisibilityChange);
    return () => document.removeEventListener('visibilitychange', handleVisibilityChange);
  }, [testId]);

  const loadTestLevels = async (testId: number) => {
    try {
      setLoading(true);
      const { data } = await http.get<TestLevel[]>(`/TestLevel/test/${testId}`);

      // Sort levels in Easy → Medium → Hard order
      const order = { Easy: 1, Medium: 2, Hard: 3 };
      const sortedLevels = data.sort((a, b) => 
        (order[a.levelName as keyof typeof order] || 999) - (order[b.levelName as keyof typeof order] || 999)
      );
      setLevels(sortedLevels);
    } catch (err) {

    } finally {
      setLoading(false);
    }
  };

  const loadCompletedLevels = async (testId: number) => {
    try {

      const response = await http.get(`/Result/user/completed/${testId}`);

      const data = response.data;

      // API returns array of level names like ["Easy", "Medium"]
      setCompletedLevels(Array.isArray(data) ? data : []);
    } catch (err: any) {

      setCompletedLevels([]);
    }
  };

  const checkCertificate = async (testId: number) => {
    try {
      const userId = getUserId();
      const { data } = await http.get(`/Certificate/user/${userId}`);
      const hasCert = data.some((cert: any) => cert.testId === testId);
      setHasCertificate(hasCert);
    } catch (err) {
      setHasCertificate(false);
    }
  };

  const getUserId = () => {
    const token = localStorage.getItem('auth_token');
    if (!token) return 0;
    try {
      const decoded: any = JSON.parse(atob(token.split('.')[1]));
      return parseInt(decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] || "0");
    } catch {
      return 0;
    }
  };

  const handleWatchVideo = (videoLink: string) => {
    if (videoLink) {
      setSelectedVideo(videoLink);
      setVideoOpen(true);
    }
  };

  const handleStartTest = (testLevelId: number) => {
    if (hasCertificate) return;
    navigate(`/user/take-test/${testLevelId}`);
  };

  const getLevelColor = (levelName: string) => {
    switch (levelName.toLowerCase()) {
      case "easy": return "success";
      case "medium": return "warning";
      case "hard": return "error";
      default: return "primary";
    }
  };

  const isLevelUnlocked = (index: number) => {
    // First level (Easy) is always unlocked
    if (index === 0) return true;
    // Other levels are unlocked if previous level is completed
    const previousLevel = levels[index - 1];
    return previousLevel && completedLevels.includes(previousLevel.levelName);
  };

  const isLevelCompleted = (levelName: string) => {
    return completedLevels.includes(levelName);
  };

  return (
    <Box sx={{ 
      background: "linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%)", 
      minHeight: "100vh" 
    }}>
      <Navbar />
      <Container maxWidth={false} sx={{ px: 4, py: 4, maxWidth: "1400px", mx: "auto" }}>
        <Typography variant="h4" fontWeight="700" gutterBottom sx={{ 
          mb: 3, 
          color: "#667eea",
          textAlign: "center",
          letterSpacing: 0.5
        }}>
          📊 Course Levels
        </Typography>

        <Alert severity="info" sx={{ mb: 4, borderRadius: 3, bgcolor: "white" }}>
          <Typography variant="body1" fontWeight="600">
            ⚠️ Important: Your final certificate score will be calculated based only on the marks achieved in the Hard level test.
          </Typography>
        </Alert>

        {loading ? (
          <Box sx={{ display: "grid", gridTemplateColumns: "repeat(auto-fit, minmax(320px, 1fr))", gap: 3 }}>
            {[1, 2, 3].map((i) => (
              <Card key={i} elevation={1} sx={{ border: "1px solid #e0e0e0", borderRadius: 3, bgcolor: "white" }}>
                <CardContent sx={{ p: 3 }}>
                  <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 2 }}>
                    <Skeleton variant="text" width="60%" height={28} />
                    <Skeleton variant="rectangular" width={60} height={24} sx={{ borderRadius: 1 }} />
                  </Box>
                  <Skeleton variant="text" width="80%" height={20} sx={{ mb: 1 }} />
                  <Skeleton variant="text" width="70%" height={20} sx={{ mb: 3 }} />
                  <Skeleton variant="rectangular" width="100%" height={36} sx={{ mb: 1, borderRadius: 1 }} />
                  <Skeleton variant="rectangular" width="100%" height={48} sx={{ borderRadius: 2 }} />
                </CardContent>
              </Card>
            ))}
          </Box>
        ) : (
          <Box sx={{ display: "grid", gridTemplateColumns: "repeat(auto-fit, minmax(320px, 1fr))", gap: 3 }}>
            {levels.map((level, index) => (
              <Card key={level.testLevelId} elevation={1} sx={{ 
                height: "100%", 
                display: "flex", 
                flexDirection: "column",
                border: "1px solid #e0e0e0",
                borderRadius: 3,
                bgcolor: "white",
                transition: "all 0.3s ease",
                "&:hover": { 
                  transform: "translateY(-4px)", 
                  boxShadow: "0 8px 25px rgba(102, 126, 234, 0.15)",
                  borderColor: "#667eea"
                }
              }}>
                <CardContent sx={{ flexGrow: 1, p: 3 }}>
                  <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 2 }}>
                    <Typography variant="h6" fontWeight="600" sx={{ color: "#667eea" }}>
                      {level.levelName}
                    </Typography>
                    <Chip 
                      label={level.levelName} 
                      color={getLevelColor(level.levelName) as any}
                      size="small"
                      sx={{ fontWeight: 600 }}
                    />
                  </Box>
                  
                  <Typography variant="body2" color="textSecondary" gutterBottom>
                    ⏱️ Duration: {level.durationMins} minutes
                  </Typography>
                  <Typography variant="body2" color="textSecondary" gutterBottom>
                    🎯 Passing Score: {level.passingScore}%
                  </Typography>

                  <Box sx={{ mt: 2, display: "flex", gap: 1, flexDirection: "column" }}>
                    {level.videoLink && (
                      <Button
                        variant="outlined"
                        startIcon={<PlayIcon />}
                        onClick={() => handleWatchVideo(level.videoLink)}
                        fullWidth
                      >
                        Watch Intro Video
                      </Button>
                    )}
                    
                    <Button
                      variant={hasCertificate ? "outlined" : "contained"}
                      onClick={() => handleStartTest(level.testLevelId)}
                      fullWidth
                      disabled={hasCertificate || !isLevelUnlocked(index)}
                      sx={{
                        py: 1.5,
                        fontWeight: 600,
                        borderRadius: 2,
                        ...(hasCertificate ? {
                          bgcolor: "#f5f5f5",
                          color: "#757575",
                          borderColor: "#e0e0e0",
                          "&:hover": { bgcolor: "#f5f5f5" }
                        } : isLevelCompleted(level.levelName) ? {
                          bgcolor: "#2e7d32",
                          "&:hover": { bgcolor: "#1b5e20" }
                        } : {
                          background: "linear-gradient(135deg, #667eea, #764ba2)",
                          "&:hover": { background: "linear-gradient(135deg, #5a67d8, #6b46c1)" }
                        })
                      }}
                    >
                      {hasCertificate 
                        ? "🏆 Certified" 
                        : !isLevelUnlocked(index) 
                          ? "🔒 Locked" 
                          : isLevelCompleted(level.levelName) 
                            ? "✅ Completed" 
                            : "Start Test"
                      }
                    </Button>
                  </Box>
                </CardContent>
                </Card>
              ))}
            </Box>
          )}

        <VideoModal
          open={videoOpen}
          onClose={() => setVideoOpen(false)}
          videoLink={selectedVideo}
        />

        <Box sx={{ mt: 4, textAlign: "center" }}>
          <Button 
            variant="outlined" 
            onClick={() => navigate("/user/dashboard")}
            sx={{ 
              px: 4, 
              py: 1.5, 
              fontWeight: 600,
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