import { 
  Box, Typography, Container, Card, CardContent, Grid, 
  Avatar, Chip, Paper, Button 
} from "@mui/material";
import { 
  School, TrendingUp, WorkspacePremium, Groups, 
  Psychology, Timeline, EmojiEvents, ArrowBack 
} from "@mui/icons-material";
import { useNavigate } from "react-router-dom";
import Navbar from "../Layout/Navbar";

export default function About() {
  const navigate = useNavigate();

  const features = [
    {
      icon: <School sx={{ fontSize: 40, color: "#667eea" }} />,
      title: "Interactive Learning",
      description: "Engage with comprehensive video tutorials and hands-on practice sessions designed by industry experts."
    },
    {
      icon: <Psychology sx={{ fontSize: 40, color: "#667eea" }} />,
      title: "Skill Assessment",
      description: "Test your knowledge with progressive difficulty levels and receive detailed performance analytics."
    },
    {
      icon: <WorkspacePremium sx={{ fontSize: 40, color: "#667eea" }} />,
      title: "Professional Certificates",
      description: "Earn industry-recognized certificates upon successful completion of all assessment levels."
    },
    {
      icon: <TrendingUp sx={{ fontSize: 40, color: "#667eea" }} />,
      title: "Progress Tracking",
      description: "Monitor your learning journey with detailed progress reports and personalized recommendations."
    }
  ];

  const journey = [
    {
      phase: "Learn",
      description: "Watch comprehensive video tutorials covering fundamental to advanced concepts",
      icon: <School />
    },
    {
      phase: "Practice",
      description: "Apply your knowledge through interactive exercises and real-world scenarios",
      icon: <Psychology />
    },
    {
      phase: "Assess",
      description: "Take progressive assessments to validate your understanding and skills",
      icon: <Timeline />
    },
    {
      phase: "Certify",
      description: "Earn professional certificates recognized by industry leaders",
      icon: <EmojiEvents />
    }
  ];

  return (
    <>
      <Navbar />
      <Box sx={{ 
        minHeight: "100vh",
        background: "linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%)"
      }}>
        <Container maxWidth="lg" sx={{ py: 4 }}>
          {/* Header */}
          <Box sx={{ mb: 6 }}>
            <Button
              startIcon={<ArrowBack />}
              onClick={() => navigate(-1)}
              sx={{ 
                mb: 4,
                color: "#667eea",
                fontWeight: 600,
                "&:hover": { bgcolor: "rgba(102, 126, 234, 0.1)" }
              }}
            >
              Back
            </Button>
            
            <Box sx={{ textAlign: "center" }}>
            
              <Typography 
                variant="h2" 
                fontWeight="800" 
                sx={{ 
                  background: "linear-gradient(135deg, #667eea, #764ba2)",
                  backgroundClip: "text",
                  WebkitBackgroundClip: "text",
                  WebkitTextFillColor: "transparent",
                  mb: 2,
                  fontSize: { xs: "2.5rem", md: "3.75rem" }
                }}
              >
                About SkillBridge
              </Typography>
              <Typography variant="h5" color="text.secondary" sx={{ 
                maxWidth: 700, 
                mx: "auto",
                fontSize: { xs: "1.1rem", md: "1.5rem" },
                lineHeight: 1.4
              }}>
                Bridging the gap between learning and professional success through innovative skill assessment
              </Typography>
            </Box>
          </Box>

          {/* Mission Statement */}
          <Paper 
            elevation={0}
            sx={{ 
              p: 4, 
              mb: 6, 
              background: "linear-gradient(135deg, #667eea 0%, #764ba2 100%)",
              color: "white",
              borderRadius: 4
            }}
          >
            <Box sx={{ textAlign: "center" }}>
              <Groups sx={{ fontSize: 60, mb: 2, opacity: 0.9 }} />
              <Typography variant="h4" fontWeight="700" gutterBottom>
                Our Mission
              </Typography>
              <Typography variant="h6" sx={{ opacity: 0.95, lineHeight: 1.6 }}>
                To empower learners worldwide by providing comprehensive skill assessment tools 
                that bridge the gap between theoretical knowledge and practical expertise, 
                enabling career advancement and professional growth.
              </Typography>
            </Box>
          </Paper>

          {/* Learning Journey */}
          <Box sx={{ mb: 8 }}>
            <Box sx={{ textAlign: "center", mb: 5 }}>
              <Typography variant="h4" fontWeight="700" gutterBottom sx={{ mb: 2 }}>
                Your Learning Journey
              </Typography>
              <Typography variant="subtitle1" color="text.secondary" sx={{ maxWidth: 600, mx: "auto" }}>
                Follow our structured approach to master new skills and advance your career
              </Typography>
            </Box>
            
            <Grid container spacing={4} sx={{ justifyContent: "center" }}>
              {journey.map((step, index) => (
                <Grid size={{ xs: 12, sm: 6, md: 3 }} key={index}>
                  <Card 
                    elevation={0}
                    sx={{ 
                      height: "100%",
                      textAlign: "center",
                      p: 3,
                      background: "linear-gradient(135deg, #ffffff 0%, #f8fafc 100%)",
                      border: "1px solid rgba(102, 126, 234, 0.1)",
                      borderRadius: 3,
                      transition: "all 0.3s ease",
                      "&:hover": {
                        transform: "translateY(-8px)",
                        boxShadow: "0 12px 40px rgba(102, 126, 234, 0.15)"
                      }
                    }}
                  >
                    <Avatar 
                      sx={{ 
                        bgcolor: "#667eea", 
                        width: 64, 
                        height: 64, 
                        mx: "auto", 
                        mb: 2 
                      }}
                    >
                      {step.icon}
                    </Avatar>
                    <Chip 
                      label={`Step ${index + 1}`} 
                      size="small" 
                      sx={{ mb: 2, bgcolor: "rgba(102, 126, 234, 0.1)", color: "#667eea" }}
                    />
                    <Typography variant="h6" fontWeight="600" gutterBottom>
                      {step.phase}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      {step.description}
                    </Typography>
                  </Card>
                </Grid>
              ))}
            </Grid>
          </Box>

          {/* Features */}
          <Box sx={{ mb: 8 }}>
            <Box sx={{ textAlign: "center", mb: 5 }}>
              <Typography variant="h4" fontWeight="700" gutterBottom sx={{ mb: 2 }}>
                Platform Features
              </Typography>
              <Typography variant="subtitle1" color="text.secondary" sx={{ maxWidth: 600, mx: "auto" }}>
                Comprehensive tools designed to enhance your learning experience
              </Typography>
            </Box>
            
            <Grid container spacing={4} sx={{ justifyContent: "center" }}>
              {features.map((feature, index) => (
                <Grid size={{ xs: 12, lg: 6 }} key={index}>
                  <Card 
                    elevation={0}
                    sx={{ 
                      p: 4,
                      height: "100%",
                      background: "linear-gradient(135deg, #ffffff 0%, #f8fafc 100%)",
                      border: "1px solid rgba(102, 126, 234, 0.1)",
                      borderRadius: 3,
                      transition: "all 0.3s ease",
                      "&:hover": {
                        transform: "translateY(-4px)",
                        boxShadow: "0 8px 30px rgba(102, 126, 234, 0.12)"
                      }
                    }}
                  >
                    <Box sx={{ display: "flex", alignItems: "flex-start", gap: 3 }}>
                      <Box sx={{ flexShrink: 0 }}>
                        {feature.icon}
                      </Box>
                      <Box>
                        <Typography variant="h6" fontWeight="600" gutterBottom>
                          {feature.title}
                        </Typography>
                        <Typography variant="body1" color="text.secondary" sx={{ lineHeight: 1.6 }}>
                          {feature.description}
                        </Typography>
                      </Box>
                    </Box>
                  </Card>
                </Grid>
              ))}
            </Grid>
          </Box>

          {/* How It Started */}
          <Paper 
            elevation={0}
            sx={{ 
              p: { xs: 4, md: 6 }, 
              background: "linear-gradient(135deg, #ffffff 0%, #f8fafc 100%)",
              border: "1px solid rgba(102, 126, 234, 0.1)",
              borderRadius: 4
            }}
          >
            <Grid container spacing={5} alignItems="center">
              <Grid size={{ xs: 12, md: 6 }}>
                <Typography variant="h4" fontWeight="700" gutterBottom>
                  How SkillBridge Started
                </Typography>
                <Typography variant="body1" color="text.secondary" sx={{ mb: 3, lineHeight: 1.7 }}>
                  SkillBridge was born from the recognition that traditional learning methods often 
                  leave a gap between theoretical knowledge and practical application. Our founders, 
                  experienced educators and industry professionals, identified the need for a platform 
                  that not only teaches but also validates skills through comprehensive assessments.
                </Typography>
                <Typography variant="body1" color="text.secondary" sx={{ lineHeight: 1.7 }}>
                  Starting as a small initiative to help students bridge their learning gaps, 
                  SkillBridge has evolved into a comprehensive platform serving thousands of learners 
                  worldwide, helping them achieve their professional goals through structured learning 
                  and skill validation.
                </Typography>
              </Grid>
              <Grid size={{ xs: 12, md: 6 }}>
                <Box 
                  sx={{ 
                    textAlign: "center",
                    p: { xs: 3, md: 4 },
                    background: "linear-gradient(135deg, #667eea 0%, #764ba2 100%)",
                    borderRadius: 3,
                    color: "white",
                    mt: { xs: 3, md: 0 }
                  }}
                >
                  <Typography variant="h2" fontWeight="800" gutterBottom sx={{ fontSize: { xs: "3rem", md: "4rem" } }}>
                    10K+
                  </Typography>
                  <Typography variant="h6" sx={{ opacity: 0.9, mb: 2 }}>
                    Learners Empowered
                  </Typography>
                  <Typography variant="body2" sx={{ opacity: 0.8 }}>
                    And growing every day
                  </Typography>
                </Box>
              </Grid>
            </Grid>
          </Paper>
        </Container>
      </Box>
    </>
  );
}