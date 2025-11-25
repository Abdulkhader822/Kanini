import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import {
  Box, Typography, Card, CardContent, Button, Radio, RadioGroup, FormControlLabel, 
  LinearProgress, Paper, Alert
} from "@mui/material";
import { toast } from 'react-toastify';
import Navbar from "../../Layout/Navbar";
import { http } from "../../Api/http";
import { tokenstore } from "../../Auth/tokenstore";

interface Question {
  questionId: number;
  questionText: string;
  optionA: string;
  optionB: string;
  optionC: string;
  optionD: string;
}

interface Answer {
  questionId: number;
  selectedOption: "A" | "B" | "C" | "D";
}

export default function TakeTest() {
  const { testLevelId } = useParams<{ testLevelId: string }>();
  const [questions, setQuestions] = useState<Question[]>([]);
  const [answers, setAnswers] = useState<Answer[]>([]);
  const [currentQuestion, setCurrentQuestion] = useState(0);
  const [testStarted, setTestStarted] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [loadingQuestions, setLoadingQuestions] = useState(true);
  const [autoSubmitted, setAutoSubmitted] = useState(false);
  const [timerRef, setTimerRef] = useState<number | null>(null);
  const [testLevel, setTestLevel] = useState<any>(null);
  const [timeLeft, setTimeLeft] = useState(0);
  const [testId, setTestId] = useState<number | null>(null);
  const [userId, setUserId] = useState<number | null>(null);
  const [startedAtUtc, setStartedAtUtc] = useState<string | null>(null);
  const navigate = useNavigate();

  useEffect(() => {
    // Extract userId from JWT token using tokenstore
    const userIdFromToken = tokenstore.getUserId();

    setUserId(userIdFromToken);

    if (testLevelId) {
      // Check for saved start time
      const saved = sessionStorage.getItem(`attemptStart_testLevel_${testLevelId}`);
      if (saved) setStartedAtUtc(saved);
    }
  }, [testLevelId]);

  // Load questions when testLevelId or userId changes
  useEffect(() => {
    if (testLevelId) {
      loadQuestions(Number(testLevelId));
    }
  }, [testLevelId, userId]);

  useEffect(() => {
    if (!startedAtUtc || !testLevel) return;

    const started = new Date(startedAtUtc).getTime();
    const expiry = started + testLevel.durationMins * 60 * 1000;
    const secondsLeft = Math.max(0, Math.ceil((expiry - Date.now()) / 1000));
    setTimeLeft(secondsLeft);

    // If time is already up, auto-submit immediately
    if (secondsLeft <= 0 && !submitting && !autoSubmitted) {

      handleAutoSubmit();
      return;
    }

    const timer = setInterval(() => {
      setTimeLeft(t => {
        const newTime = Math.max(0, t - 1);
        if (newTime <= 0 && !submitting && !autoSubmitted) {

          handleAutoSubmit();
        }
        return newTime;
      });
    }, 1000);
    setTimerRef(timer);
    return () => {
      clearInterval(timer);
      setTimerRef(null);
    };
  }, [startedAtUtc, testLevel, submitting, autoSubmitted]);

  const loadQuestions = async (testLevelId: number) => {
    setLoadingQuestions(true);
    try {

      
      // Get test level info first to get testId
      const levelResponse = await http.get(`/TestLevel/details/${testLevelId}`);
      const levelData = levelResponse.data;
      setTestLevel(levelData);
      setTestId(levelData.testId);
      setTimeLeft(levelData.durationMins * 60);
      

      
      // Get questions with user-specific ordering if userId is available
      if (userId && levelData.testId) {
        try {

          const questionResponse = await http.get(`/Question/user/${levelData.testId}/${testLevelId}/${userId}`);

          
          if (questionResponse.data.questions && questionResponse.data.questions.length > 0) {
            setQuestions(questionResponse.data.questions);

            return;
          }
        } catch (userEndpointError) {
          // User-specific endpoint failed, falling back to regular endpoint
        }
      }
      
      // Fallback to regular endpoint

      const fallbackResponse = await http.get(`/Question/testlevel/${testLevelId}`);

      setQuestions(fallbackResponse.data);

      
    } catch (err) {

      toast.error("Failed to load questions. Please try again.");
    } finally {
      setLoadingQuestions(false);
    }
  };

  const startTest = () => {
    const utc = new Date().toISOString();

    setStartedAtUtc(utc);
    sessionStorage.setItem(`attemptStart_testLevel_${testLevelId}`, utc);
    setTestStarted(true);
  };

  const handleAnswerChange = (questionId: number, selectedOption: "A" | "B" | "C" | "D") => {
    setAnswers(prev => {
      const existing = prev.find(a => a.questionId === questionId);
      if (existing) {
        return prev.map(a => a.questionId === questionId ? { ...a, selectedOption } : a);
      }
      return [...prev, { questionId, selectedOption }];
    });
  };

  const handleSubmit = async () => {
    if (submitting || !userId || !testId || !startedAtUtc || autoSubmitted) return;
    
    // Clear timer immediately to prevent auto-submit
    if (timerRef) {
      clearInterval(timerRef);
      setTimerRef(null);
    }
    
    setSubmitting(true);
    setAutoSubmitted(true); // Prevent auto-submit after manual submit

    try {
      const startTime = new Date(startedAtUtc).getTime();
      const currentTime = Date.now();
      const timeTaken = Math.floor((currentTime - startTime) / 1000);
      
      const payload = {
        userId: Number(userId),
        testId: Number(testId),
        testLevelId: Number(testLevelId),
        startedAtUtc: new Date(startedAtUtc).toISOString(),
        timeTakenSecs: timeTaken,
        answers: questions.map((q) => ({
          questionId: q.questionId,
          selectedOption: answers.find(a => a.questionId === q.questionId)?.selectedOption || "",
        })),
      };

      const response = await http.post("/Result/submit", payload);
      
      // Extract resultId from response - check multiple possible locations
      const resultId = response.data.resultId || response.data.result?.resultId;
      
      // Clear session storage
      sessionStorage.removeItem(`attemptStart_testLevel_${testLevelId}`);
      
      // Show success toast
      toast.success("Test submitted successfully!");
      
      // Delay navigation to ensure toast is visible
      setTimeout(() => {
        if (resultId && !isNaN(Number(resultId))) {
          navigate(`/user/result/${resultId}`);
        } else {
          navigate("/user/dashboard");
        }
      }, 1500);
    } catch (err: any) {
      // Clear session storage even on error
      sessionStorage.removeItem(`attemptStart_testLevel_${testLevelId}`);
      
      if (err.response?.status === 400) {
        const errorMsg = err.response?.data?.error || "Test submission failed";
        toast.error(`Submit failed: ${errorMsg}`);
      } else {
        toast.error("Test submission failed. Please try again.");
      }
      setSubmitting(false);
      setAutoSubmitted(false);
    }
  };

  const handleAutoSubmit = async () => {
    if (submitting || !userId || !testId || !startedAtUtc || autoSubmitted) {
      return;
    }
    
    // Clear timer immediately to prevent multiple calls
    if (timerRef) {
      clearInterval(timerRef);
      setTimerRef(null);
    }
    
    setSubmitting(true);
    setAutoSubmitted(true);

    try {
      const startTime = new Date(startedAtUtc).getTime();
      const currentTime = Date.now();
      const timeTaken = Math.floor((currentTime - startTime) / 1000);
      
      const payload = {
        userId: Number(userId),
        testId: Number(testId),
        testLevelId: Number(testLevelId),
        startedAtUtc: new Date(startedAtUtc).toISOString(),
        timeTakenSecs: timeTaken,
        answers: questions.map((q) => ({
          questionId: q.questionId,
          selectedOption: answers.find(a => a.questionId === q.questionId)?.selectedOption || "",
        })),
      };

      const response = await http.post("/Result/submit", payload);
      
      // Extract resultId from response - check multiple possible locations
      const resultId = response.data.resultId || response.data.result?.resultId;
      
      // Clear session storage
      sessionStorage.removeItem(`attemptStart_testLevel_${testLevelId}`);
      
      if (resultId && !isNaN(Number(resultId))) {
        navigate(`/user/result/${resultId}`);
      } else {
        navigate("/user/dashboard");
      }
    } catch (err: any) {
      // Clear session storage even on error
      sessionStorage.removeItem(`attemptStart_testLevel_${testLevelId}`);
      
      if (err.response?.status === 400) {
        const errorMsg = err.response?.data?.error || "Test submission failed";
        toast.error(`Auto-submit failed: ${errorMsg}`);
      } else {
        toast.warning("Time's up! Test time expired.");
      }
      navigate("/user/dashboard");
    }
  };

  const formatTime = (seconds: number) => {
    const mins = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${mins}:${secs.toString().padStart(2, '0')}`;
  };

  if (!testStarted) {
    return (
      <>
        <Navbar />
        <Box sx={{ maxWidth: 800, mx: "auto", p: 3, textAlign: "center" }}>
          <Card sx={{ p: 4 }}>
            <Typography variant="h5" fontWeight="bold" gutterBottom>
              🧠 Ready to Start Test?
            </Typography>
            <Typography variant="body1" gutterBottom sx={{ mb: 3 }}>
              You have {testLevel?.durationMins || 0} minutes to complete {questions.length} questions.
            </Typography>
            <Alert severity="info" sx={{ mb: 3 }}>
              Once you start, the timer will begin automatically. Make sure you have a stable internet connection.
            </Alert>
            <Button variant="contained" size="large" onClick={startTest}>
              Start Test
            </Button>
          </Card>
        </Box>
      </>
    );
  }

  if (loadingQuestions || questions.length === 0) {
    return (
      <>
        <Navbar />
        <Box sx={{ maxWidth: 800, mx: "auto", p: 3, textAlign: "center" }}>
          <Typography variant="h6">
            {loadingQuestions ? "Loading questions..." : "No questions available for this test level."}
          </Typography>
          {!loadingQuestions && questions.length === 0 && (
            <Button 
              variant="outlined" 
              onClick={() => navigate(-1)}
              sx={{ mt: 2 }}
            >
              Go Back
            </Button>
          )}
        </Box>
      </>
    );
  }

  const question = questions[currentQuestion];
  const currentAnswer = answers.find(a => a.questionId === question.questionId)?.selectedOption || "";

  return (
    <>
      <Navbar />
      <Box sx={{ maxWidth: 800, mx: "auto", p: 3 }}>
        {/* Timer and Progress */}
        <Paper sx={{ p: 2, mb: 3 }}>
          <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 1 }}>
            <Typography variant="h6">
              Question {currentQuestion + 1} of {questions.length}
            </Typography>
            <Typography variant="h6" color={timeLeft < 300 ? "error" : "primary"}>
              ⏰ {formatTime(timeLeft)}
            </Typography>
          </Box>
          <LinearProgress variant="determinate" value={testLevel ? ((testLevel.durationMins * 60 - timeLeft) / (testLevel.durationMins * 60)) * 100 : 0} />
        </Paper>

        {/* Question */}
        <Card sx={{ mb: 3 }}>
          <CardContent>
            <Typography variant="h6" gutterBottom>
              {question.questionText}
            </Typography>
            
            <RadioGroup
              value={currentAnswer}
              onChange={(e) => handleAnswerChange(question.questionId, e.target.value as "A" | "B" | "C" | "D")}
            >
              <FormControlLabel value="A" control={<Radio />} label={`A. ${question.optionA}`} />
              <FormControlLabel value="B" control={<Radio />} label={`B. ${question.optionB}`} />
              <FormControlLabel value="C" control={<Radio />} label={`C. ${question.optionC}`} />
              <FormControlLabel value="D" control={<Radio />} label={`D. ${question.optionD}`} />
            </RadioGroup>
          </CardContent>
        </Card>

        {/* Navigation */}
        <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
          <Button
            variant="outlined"
            onClick={() => setCurrentQuestion(prev => Math.max(0, prev - 1))}
            disabled={currentQuestion === 0}
          >
            Previous
          </Button>

          <Typography variant="body2" color="textSecondary">
            {answers.length} of {questions.length} answered
          </Typography>

          {currentQuestion < questions.length - 1 ? (
            <Button
              variant="contained"
              onClick={() => setCurrentQuestion(prev => Math.min(questions.length - 1, prev + 1))}
            >
              Next
            </Button>
          ) : (
            <Button
              variant="contained"
              color="success"
              onClick={handleSubmit}
              disabled={submitting || !userId || !testId}
            >
              {submitting ? "Submitting..." : "Submit Test"}
            </Button>
          )}
        </Box>
      </Box>
    </>
  );
}