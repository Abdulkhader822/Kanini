import { Box } from '@mui/material';
import Navbar from './Navbar';
import Footer from './Footer';

interface LayoutProps {
  children: React.ReactNode;
}

const layoutStyles = {
  display: 'flex',
  flexDirection: 'column',
  minHeight: '100vh'
};

const mainStyles = {
  flexGrow: 1,
  backgroundColor: '#F5F7FA'
};

export default function Layout({ children }: LayoutProps) {
  return (
    <Box sx={layoutStyles}>
      <Navbar />
      <Box component="main" sx={mainStyles}>
        {children}
      </Box>
      <Footer />
    </Box>
  );
}