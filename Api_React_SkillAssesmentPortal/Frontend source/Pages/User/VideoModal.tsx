import { Dialog, DialogContent, DialogTitle, IconButton, Box,Typography} from "@mui/material";
import { Close as CloseIcon } from "@mui/icons-material";

interface VideoModalProps {
  open: boolean;
  onClose: () => void;
  videoLink: string;
}

export default function VideoModal({ open, onClose, videoLink }: VideoModalProps) {
  const getYouTubeEmbedUrl = (url: string) => {
    if (!url) return "";
    
    // Clean the URL
    const cleanUrl = url.trim();
    
    // Handle various YouTube URL formats
    const patterns = [
      /(?:youtube\.com\/watch\?v=)([a-zA-Z0-9_-]{11})/,
      /(?:youtu\.be\/)([a-zA-Z0-9_-]{11})/,
      /(?:youtube\.com\/embed\/)([a-zA-Z0-9_-]{11})/,
      /(?:youtube\.com\/v\/)([a-zA-Z0-9_-]{11})/,
      /(?:youtube\.com\/.*[?&]v=)([a-zA-Z0-9_-]{11})/
    ];
    
    for (const pattern of patterns) {
      const match = cleanUrl.match(pattern);
      if (match && match[1]) {
        return `https://www.youtube.com/embed/${match[1]}?autoplay=0&rel=0&modestbranding=1&enablejsapi=1`;
      }
    }
    
    // If it's already a valid embed URL, ensure proper parameters
    if (cleanUrl.includes('youtube.com/embed/')) {
      const videoId = cleanUrl.match(/embed\/([a-zA-Z0-9_-]{11})/);
      if (videoId && videoId[1]) {
        return `https://www.youtube.com/embed/${videoId[1]}?autoplay=0&rel=0&modestbranding=1&enablejsapi=1`;
      }
    }
    
    // Return original URL if no pattern matches
    return cleanUrl;
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="md" fullWidth>
      <DialogTitle sx={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
        Level Introduction Video
        <IconButton onClick={onClose}>
          <CloseIcon />
        </IconButton>
      </DialogTitle>
      <DialogContent sx={{ p: 0 }}>
        {videoLink ? (
          <Box sx={{ position: "relative", paddingBottom: "56.25%", height: 0, bgcolor: "#000" }}>
            <iframe
              src={getYouTubeEmbedUrl(videoLink)}
              style={{
                position: "absolute",
                top: 0,
                left: 0,
                width: "100%",
                height: "100%",
                border: "none"
              }}
              allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
              allowFullScreen
              title="Level Introduction Video"
            />
          </Box>
        ) : (
          <Box sx={{ p: 4, textAlign: "center" }}>
            <Typography variant="h6" color="textSecondary">
              Video not available
            </Typography>
          </Box>
        )}
      </DialogContent>
    </Dialog>
  );
}