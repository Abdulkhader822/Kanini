import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import {
  Box, Typography, Card, CardContent, Button, Chip, Alert, LinearProgress
} from "@mui/material";
import { toast } from 'react-toastify';
import Navbar from "../../Layout/Navbar";
import { http } from "../../Api/http";
import { tokenstore } from "../../Auth/tokenstore";

interface Result {
  resultId: number;
  testLevelId: number;
  score: number;
  resultStatus?: string;
  feedback: string;
  testName: string;
  levelName: string;
  passingScore?: number;
  testId?: number;
}

interface DetailedResult {
  resultId: number;
  userId: number;
  userName: string;
  testId: number;
  testName: string;
  testLevelId: number;
  levelName: string;
  passingScore: number;
  score: number;
  percentage: number;
  resultStatus: string;
  dateAttempted: string;
  attemptNumber: number;
  suggestion: string;
  questions: QuestionResult[];
}

interface QuestionResult {
  questionId: number;
  questionText: string;
  optionA: string;
  optionB: string;
  optionC: string;
  optionD: string;
  correctOption: string;
  selectedOption: string;
  isCorrect: boolean;
}

interface Certificate {
  certificateId: number;
  userId: number;
  testId: number;
  issuedDate: string;
  certificateUrl: string;
}

export default function ResultPage() {
  const { resultId } = useParams<{ resultId: string }>();
  const [result, setResult] = useState<Result | null>(null);
  const [detailedResult, setDetailedResult] = useState<DetailedResult | null>(null);
  const [showDetails, setShowDetails] = useState(false);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [certificate, setCertificate] = useState<Certificate | null>(null);
  const [checkingCertificate, setCheckingCertificate] = useState(false);
  const navigate = useNavigate();

  useEffect(() => {
    if (!resultId || isNaN(Number(resultId))) {

      navigate("/user/dashboard");
      return;
    }

    const fetchResult = async () => {
      try {

        const { data } = await http.get<Result>(`/Result/${resultId}`);
        setResult(data);

        
        // If passed, check for certificate availability
        const isPassed = data.resultStatus?.toLowerCase() === "pass";
        if (isPassed && data.testId) {
          await checkCertificate(data.testId);
        }
      } catch (error) {

        setError("Result not found. Please try again.");
      } finally {
        setLoading(false);
      }
    };

    const fetchDetailedResult = async () => {
      try {
        const { data } = await http.get<DetailedResult>(`/Result/${resultId}/detailed`);
        setDetailedResult(data);
      } catch (error) {

      }
    };

    fetchResult();
    fetchDetailedResult();
  }, [resultId, navigate]);

  const checkCertificate = async (testId: number) => {
    setCheckingCertificate(true);
    try {
      const userId = tokenstore.getUserId();
      if (userId) {
        const { data } = await http.get<Certificate[]>(`/Certificate/user/${userId}`);
        const cert = Array.isArray(data) ? data.find(c => c.testId === testId) : null;
        if (cert) {
          setCertificate(cert);
        }
      }
    } catch (err) {

    } finally {
      setCheckingCertificate(false);
    }
  };

  const handleDownloadCertificate = async () => {
    if (!result?.testId) return;
    const userId = tokenstore.getUserId();
    if (!userId) return;
    
    try {
      const response = await http.get(`/Result/certificate/download/${userId}/${result.testId}`, {
        responseType: 'blob'
      });
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', `Certificate_${result.testName}.pdf`);
      document.body.appendChild(link);
      link.click();
      link.remove();
    } catch (error) {

      alert('Failed to download certificate. Please try again.');
    }
  };

  const handleRetakeTest = () => {
    if (result) {
      navigate(`/user/take-test/${result.testLevelId}`);
    }
  };

  const handleBackToDashboard = () => {
    navigate("/user/dashboard");
  };

  const handleBackToLevels = () => {
    if (result?.testId) {
      // Trigger refresh of completed levels
      localStorage.setItem('testCompleted', 'true');
      navigate(`/user/test-levels/${result.testId}`);
    } else {
      navigate("/user/dashboard");
    }
  };

  // Simple fast-loading fallback
  if (loading) return <div>Loading result...</div>;

  if (error || !result) {
    return (
      <>
        <Navbar />
        <Box sx={{ maxWidth: 800, mx: "auto", p: 3, textAlign: "center" }}>
          <Alert severity="error">
            {error || "Result not found. Please try again."}
          </Alert>
          <Button variant="outlined" onClick={handleBackToDashboard} sx={{ mt: 2 }}>
            Back to Dashboard
          </Button>
        </Box>
      </>
    );
  }

  const isPassed = result.resultStatus?.toLowerCase() === "pass";

  return (
    <>
      <Navbar />
      <Box sx={{ maxWidth: 800, mx: "auto", p: 3 }}>
        <Card sx={{ mb: 3, textAlign: "center" }}>
          <CardContent sx={{ p: 4 }}>
            <div className={`text-${isPassed ? "green" : "red"}-600 text-3xl font-bold`}>
              {isPassed ? "Congratulations!" : "Keep Learning!"}
            </div>
            
            <Typography variant="h6" gutterBottom sx={{ mt: 2 }}>
              {isPassed 
                ? `You passed the ${result.testName} - ${result.levelName} level!`
                : "You didn't pass this time, but don't give up!"
              }
            </Typography>

            <Box sx={{ my: 3 }}>
              <Typography variant="h3" fontWeight="bold" color={isPassed ? "success.main" : "error.main"}>
                {result.score}%
              </Typography>
              <p className="text-lg">
                Passing Score: {result.passingScore ?? "-"}%
              </p>
            </Box>

            <Box sx={{ mb: 3 }}>
              <LinearProgress
                variant="determinate"
                value={result.score}
                sx={{ height: 10, borderRadius: 5 }}
                color={isPassed ? "success" : "error"}
              />
            </Box>

            <Chip
              label={isPassed ? "PASSED" : "FAILED"}
              color={isPassed ? "success" : "error"}
              size="medium"
              sx={{ mb: 2, fontSize: '1.1rem', px: 2, py: 1 }}
            />

            {result.feedback && (
              <Alert severity={isPassed ? "success" : "info"} sx={{ mt: 2, textAlign: "left" }}>
                <Typography variant="body1">
                  {result.feedback}
                </Typography>
              </Alert>
            )}
          </CardContent>
        </Card>

        {/* Action Buttons */}
        <Box sx={{ display: "flex", gap: 2, justifyContent: "center", flexWrap: "wrap" }}>
          {isPassed ? (
            <>
              {certificate ? (
                <Button
                  variant="contained"
                  color="success"
                  onClick={handleDownloadCertificate}
                  size="large"
                >
                  🏆 Download Certificate
                </Button>
              ) : (
                <Button
                  variant="outlined"
                  disabled={checkingCertificate}
                  size="large"
                >
                  {checkingCertificate ? "Generating Certificate..." : "Certificate Pending"}
                </Button>
              )}
              <Button
                variant="outlined"
                onClick={handleBackToLevels}
                size="large"
              >
                Back to Levels
              </Button>
              <Button
                variant="outlined"
                onClick={handleBackToDashboard}
                size="large"
              >
                Dashboard
              </Button>
            </>
          ) : (
            <>
              <Button
                variant="contained"
                onClick={handleRetakeTest}
                size="large"
              >
                🔄 Retake Test
              </Button>
              <Button
                variant="outlined"
                onClick={handleBackToLevels}
                size="large"
              >
                Back to Levels
              </Button>
              <Button
                variant="outlined"
                onClick={handleBackToDashboard}
                size="large"
              >
                Dashboard
              </Button>
            </>
          )}
        </Box>

        {/* Test Details */}
        <Card sx={{ mt: 3 }}>
          <CardContent>
            <Typography variant="h6" gutterBottom>
              📊 Test Details
            </Typography>
            <Typography variant="body1" gutterBottom>
              <strong>Test:</strong> {result.testName}
            </Typography>
            <Typography variant="body1" gutterBottom>
              <strong>Level:</strong> {result.levelName}
            </Typography>
            <Typography variant="body1" gutterBottom>
              <strong>Your Score:</strong> {result.score}%
            </Typography>
            <Typography variant="body1" gutterBottom>
              <strong>Required Score:</strong> {result.passingScore ?? "-"}%
            </Typography>
            
            {detailedResult && (
              <Button
                variant="outlined"
                onClick={() => setShowDetails(!showDetails)}
                sx={{ mt: 2 }}
              >
                {showDetails ? "Hide" : "Show"} Question Details
              </Button>
            )}
          </CardContent>
        </Card>

        {/* Detailed Results */}
        {showDetails && detailedResult && (
          <Card sx={{ mt: 3 }}>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                📝 Question-wise Results
              </Typography>
              {detailedResult.questions.map((q, index) => (
                <Card 
                  key={q.questionId} 
                  sx={{ 
                    mb: 2, 
                    border: q.isCorrect ? "2px solid #4caf50" : "2px solid #f44336",
                    bgcolor: q.isCorrect ? "rgba(76, 175, 80, 0.05)" : "rgba(244, 67, 54, 0.05)"
                  }}
                >
                  <CardContent>
                    <Typography variant="subtitle1" fontWeight="600" gutterBottom>
                      Q{index + 1}. {q.questionText}
                    </Typography>
                    
                    <Box sx={{ ml: 2, mb: 2 }}>
                      <Typography variant="body2" sx={{ 
                        color: q.selectedOption === "A" ? (q.isCorrect ? "success.main" : "error.main") : "text.secondary",
                        fontWeight: q.correctOption === "A" ? 600 : 400
                      }}>
                        A. {q.optionA} {q.correctOption === "A" && "✓"} {q.selectedOption === "A" && !q.isCorrect && "✗"}
                      </Typography>
                      <Typography variant="body2" sx={{ 
                        color: q.selectedOption === "B" ? (q.isCorrect ? "success.main" : "error.main") : "text.secondary",
                        fontWeight: q.correctOption === "B" ? 600 : 400
                      }}>
                        B. {q.optionB} {q.correctOption === "B" && "✓"} {q.selectedOption === "B" && !q.isCorrect && "✗"}
                      </Typography>
                      <Typography variant="body2" sx={{ 
                        color: q.selectedOption === "C" ? (q.isCorrect ? "success.main" : "error.main") : "text.secondary",
                        fontWeight: q.correctOption === "C" ? 600 : 400
                      }}>
                        C. {q.optionC} {q.correctOption === "C" && "✓"} {q.selectedOption === "C" && !q.isCorrect && "✗"}
                      </Typography>
                      <Typography variant="body2" sx={{ 
                        color: q.selectedOption === "D" ? (q.isCorrect ? "success.main" : "error.main") : "text.secondary",
                        fontWeight: q.correctOption === "D" ? 600 : 400
                      }}>
                        D. {q.optionD} {q.correctOption === "D" && "✓"} {q.selectedOption === "D" && !q.isCorrect && "✗"}
                      </Typography>
                    </Box>
                    
                    <Box sx={{ display: "flex", gap: 2, alignItems: "center" }}>
                      <Chip 
                        label={q.isCorrect ? "Correct" : "Incorrect"} 
                        color={q.isCorrect ? "success" : "error"} 
                        size="small"
                      />
                      <Typography variant="body2" color="text.secondary">
                        Your answer: <strong>{q.selectedOption || "Not answered"}</strong>
                      </Typography>
                      <Typography variant="body2" color="text.secondary">
                        Correct answer: <strong>{q.correctOption}</strong>
                      </Typography>
                    </Box>
                  </CardContent>
                </Card>
              ))}
            </CardContent>
          </Card>
        )}
      </Box>
    </>
  );
}