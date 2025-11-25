import { Box, Typography, Link, Container, Divider } from '@mui/material';
import { Email, Phone, LocationOn, School, Assessment, WorkspacePremium } from '@mui/icons-material';

export default function Footer() {
  return (
    <Box
      component="footer"
      sx={{
        background: 'linear-gradient(135deg, #263238 0%, #37474f 100%)',
        color: '#ffffff',
        py: 3,
        mt: 'auto',
      }}
    >
      <Container maxWidth="lg">
        <Box sx={{ display: 'flex', flexWrap: 'nowrap', gap: 3, alignItems: 'flex-start' }}>
          {/* Company Info */}
          <Box sx={{ flex: '1 1 30%', minWidth: 200 }}>
            <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
              <Assessment sx={{ mr: 1, color: '#4DB6AC' }} />
              <Typography variant="h6" fontWeight="700" sx={{ color: '#4DB6AC' }}>
                SkillBridge
              </Typography>
            </Box>
            <Typography variant="body2" sx={{ mb: 2, color: '#B0BEC5', lineHeight: 1.6 }}>
              Bridging the gap between learning and professional success through innovative skill assessment. 
              Build your expertise, validate your knowledge, and advance your career.
            </Typography>
            <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
              <Email sx={{ mr: 1, fontSize: 18, color: '#4DB6AC' }} />
              <Typography variant="body2" sx={{ color: '#B0BEC5' }}>
                support@skillbridge.com
              </Typography>
            </Box>
            <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
              <Phone sx={{ mr: 1, fontSize: 18, color: '#4DB6AC' }} />
              <Typography variant="body2" sx={{ color: '#B0BEC5' }}>
                +1 (555) 123-4567
              </Typography>
            </Box>
            <Box sx={{ display: 'flex', alignItems: 'center' }}>
              <LocationOn sx={{ mr: 1, fontSize: 18, color: '#4DB6AC' }} />
              <Typography variant="body2" sx={{ color: '#B0BEC5' }}>
                123 Education Street, Learning City
              </Typography>
            </Box>
          </Box>

          {/* Quick Links */}
          <Box sx={{ flex: '1 1 15%', minWidth: 120 }}>
            <Typography variant="h6" fontWeight="600" sx={{ mb: 2, color: '#4DB6AC' }}>
              Quick Links
            </Typography>
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1 }}>
              <Link href="/about" color="inherit" underline="hover" sx={{ color: '#B0BEC5', '&:hover': { color: '#4DB6AC' } }}>
                About Us
              </Link>
              <Link href="/tests" color="inherit" underline="hover" sx={{ color: '#B0BEC5', '&:hover': { color: '#4DB6AC' } }}>
                Available Tests
              </Link>
              <Link href="/certificates" color="inherit" underline="hover" sx={{ color: '#B0BEC5', '&:hover': { color: '#4DB6AC' } }}>
                Certifications
              </Link>
              <Link href="/help" color="inherit" underline="hover" sx={{ color: '#B0BEC5', '&:hover': { color: '#4DB6AC' } }}>
                Help Center
              </Link>
            </Box>
          </Box>

          {/* Features */}
          <Box sx={{ flex: '1 1 20%', minWidth: 160 }}>
            <Typography variant="h6" fontWeight="600" sx={{ mb: 2, color: '#4DB6AC' }}>
              Features
            </Typography>
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1 }}>
              <Box sx={{ display: 'flex', alignItems: 'center' }}>
                <School sx={{ mr: 1, fontSize: 16, color: '#4DB6AC' }} />
                <Typography variant="body2" sx={{ color: '#B0BEC5' }}>
                  Multi-level Assessments
                </Typography>
              </Box>
              <Box sx={{ display: 'flex', alignItems: 'center' }}>
                <WorkspacePremium sx={{ mr: 1, fontSize: 16, color: '#4DB6AC' }} />
                <Typography variant="body2" sx={{ color: '#B0BEC5' }}>
                  Digital Certificates
                </Typography>
              </Box>
              <Box sx={{ display: 'flex', alignItems: 'center' }}>
                <Assessment sx={{ mr: 1, fontSize: 16, color: '#4DB6AC' }} />
                <Typography variant="body2" sx={{ color: '#B0BEC5' }}>
                  Real-time Results
                </Typography>
              </Box>
              <Typography variant="body2" sx={{ color: '#B0BEC5', ml: 3 }}>
                Progress Tracking
              </Typography>
            </Box>
          </Box>

          {/* Legal & Support */}
          <Box sx={{ flex: '1 1 20%', minWidth: 160 }}>
            <Typography variant="h6" fontWeight="600" sx={{ mb: 2, color: '#4DB6AC' }}>
              Legal & Support
            </Typography>
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1 }}>
              <Link href="/privacy" color="inherit" underline="hover" sx={{ color: '#B0BEC5', '&:hover': { color: '#4DB6AC' } }}>
                Privacy Policy
              </Link>
              <Link href="/terms" color="inherit" underline="hover" sx={{ color: '#B0BEC5', '&:hover': { color: '#4DB6AC' } }}>
                Terms of Service
              </Link>
              <Link href="/contact" color="inherit" underline="hover" sx={{ color: '#B0BEC5', '&:hover': { color: '#4DB6AC' } }}>
                Contact Support
              </Link>
              <Link href="/faq" color="inherit" underline="hover" sx={{ color: '#B0BEC5', '&:hover': { color: '#4DB6AC' } }}>
                FAQ
              </Link>
            </Box>
          </Box>
        </Box>

        <Divider sx={{ my: 2, borderColor: '#37474f' }} />
        
        <Box
          sx={{
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center',
            flexWrap: 'wrap',
            gap: 2,
          }}
        >
          <Typography variant="body2" sx={{ color: '#B0BEC5' }}>
            © {new Date().getFullYear()} SkillBridge. All rights reserved. | Bridge Your Skills to Success
          </Typography>
          <Typography variant="body2" sx={{ color: '#4DB6AC', fontWeight: 500 }}>
            Version 2.0.1
          </Typography>
        </Box>
      </Container>
    </Box>
  );
}