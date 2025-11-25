import { createTheme } from '@mui/material/styles';

export const theme = createTheme({
  palette: {
    primary: {
      main: '#00796B', // Deep Teal
      light: '#4DB6AC', // Secondary Teal
      dark: '#004D40',
    },
    secondary: {
      main: '#FFB300', // Rich Gold/Amber
      light: '#FFCC02',
      dark: '#FF8F00',
    },
    success: {
      main: '#388E3C', // Formal Green
      light: '#66BB6A',
      dark: '#2E7D32',
    },
    background: {
      default: '#F5F7FA', // Subtle background
      paper: '#FFFFFF', // Pure white for cards
    },
    text: {
      primary: '#37474F', // Warm Gray
      secondary: '#607D8B',
    },
    divider: '#E0E0E0',
  },
  components: {
    MuiButton: {
      styleOverrides: {
        root: {
          textTransform: 'none',
          borderRadius: 8,
          fontWeight: 600,
        },
        containedPrimary: {
          backgroundColor: '#00796B',
          '&:hover': {
            backgroundColor: '#FFB300',
          },
        },
        containedSecondary: {
          backgroundColor: '#388E3C',
          '&:hover': {
            backgroundColor: '#FFB300',
          },
        },
      },
    },
    MuiCard: {
      styleOverrides: {
        root: {
          backgroundColor: '#FFFFFF',
          borderRadius: 12,
          boxShadow: '0 2px 8px rgba(0,0,0,0.1)',
        },
      },
    },
    MuiAppBar: {
      styleOverrides: {
        root: {
          backgroundColor: '#00796B',
        },
      },
    },
    MuiPaper: {
      styleOverrides: {
        root: {
          backgroundColor: '#FFFFFF',
        },
      },
    },
  },
});