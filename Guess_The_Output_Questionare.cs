﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using AlgoQuest;
using MySql.Data.MySqlClient;
using static System.Console;
using static System.Net.Mime.MediaTypeNames;

using System.Data.SqlClient;
using Google.Protobuf.WellKnownTypes;
using System.Net.Sockets;
using Google.Protobuf;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Threading;
using NAudio.Wave;
using System.Media;
using System.IO;
using Org.BouncyCastle.Tls.Crypto.Impl.BC;
using Mysqlx.Crud;
using System.Collections;

namespace AlgoQuest
{
    class Guess_The_Output_Questionare
    {
        int id = 0;

        int holderPoints = 0;
        int Eattempt = 1, Mattempt = 1, Hattempt = 1;
        string isSelected = "";
        string check = " ";
        int easyPoints = 0, mediumPoints = 0, hardPoints = 0;
        int easyCounter = 0, mediumCounter = 0, hardCounter = 0;



        string mycon = "datasource=localhost;Database=algoquest;username=root;convert zero datetime=true";

        //-------------------------------------- CALLING OTHER CLASS -------------------------------------------------



        Title myTitles = new Title();

        game myGame = new game();

        ProcessOfModesSelection pms = new ProcessOfModesSelection();



        //---------------------------------------------------------------------------------------







        public void easyGuessQuestionaire()
        {

        tryEasyAgain:
            int counter = 0;
            int c = 0;

            while (counter < 11)
            {

                try
                {
                    // Query to get a random IDQ
                    string randomIdQuery = "SELECT IDEQGUESS FROM tbeasyquestion_guess ORDER BY RAND() LIMIT 1;";
                    int randomIDQ = 0;
                    bool isUniqueIDFound = false;
                    HashSet<int> usedIds = new HashSet<int>();  

                    using (MySqlConnection myConn = new MySqlConnection(mycon))
                    {
                        MySqlCommand randomIdCommand = new MySqlCommand(randomIdQuery, myConn);

                        myConn.Open();
                        object result = randomIdCommand.ExecuteScalar();
                        if (result != null)
                        {
                            randomIDQ = Convert.ToInt32(result);
                            if (!usedIds.Contains(randomIDQ))
                            {
                                isUniqueIDFound = true;
                                usedIds.Add(randomIDQ);  
                            }
                        }
                        else
                        {
                            Console.WriteLine("No questions available in the database.");
                            return;
                        }
                    }

                    // Query to fetch the question with the random IDQ
                    string query = "SELECT IDEQGUESS, QUESTION, ANSWER, A, B FROM  tbeasyquestion_guess WHERE IDEQGUESS = @IDEQGUESS";

                    using (MySqlConnection myConn = new MySqlConnection(mycon))
                    {
                        MySqlCommand myCommand = new MySqlCommand(query, myConn);
                        myCommand.Parameters.AddWithValue("@IDEQGUESS", randomIDQ);

                        myConn.Open();

                        using (MySqlDataReader reader = myCommand.ExecuteReader())
                        {
                            if (reader.Read()) // Check if a row is returned
                            {
                                string question = reader["QUESTION"].ToString().Replace("  ", "\n\t\t\t\t\t\t\t\t\t");
                                string choiceA = reader["A"].ToString();
                                string choiceB = reader["B"].ToString();
                                string correctAnswer = reader["ANSWER"].ToString(); // "A" or "B"

                                // Display the question and choices
                                int selectedOption = 0;
                                bool answerSelected = false;


                                while (!answerSelected)
                                {
                                    Console.Clear();
                                    myTitles.actualGameTitle();
                                    isSelected = "GuessEasy";
                                    actualGameGuess();
                                    if (c < 10)
                                    {
                                        ResetColor();
                                        Console.BackgroundColor = ConsoleColor.DarkRed;
                                        Console.WriteLine("\t\t\t\t\t\t\t\tInstructions: Analyze the code, compute the result step by step using the correct order of operations, \n\t\t\t\t\t\t\t\t\tand compare it with the given output to determine if it is correct or not.");
                                        Console.WriteLine($"\n\n\t\t\t\t\t\t\t\t\tQUESTION {counter + 1} - QUESTION 10\n");
                                          Console.WriteLine($"\n\t\t\t\t\t\t\t\t\t{question}\n");
                                        Console.WriteLine($"\t\t\t\t\t\t\t\t\t{correctAnswer}\n");
                                        Console.WriteLine();
                                        Console.WriteLine();
                                        Console.ForegroundColor = ConsoleColor.White;

                                        // Display choices directly, no "A" or "B" prefix
                                        string[] choices = { choiceA, choiceB };
                                        for (int j = 0; j < choices.Length; j++)

                                        {
                                            if (j == selectedOption)
                                            {
                                                Console.BackgroundColor = ConsoleColor.DarkRed;
                                                Console.ForegroundColor = ConsoleColor.Yellow;
                                            }
                                            else
                                            {
                                                Console.ResetColor();
                                            }

                                            Console.BackgroundColor = ConsoleColor.DarkRed;
                                            Console.WriteLine($"\t\t\t\t\t\t\t\t\t\t\t\t\t{choices[j]}");
                                        }

                                        // Key input for navigation and selection
                                        var key = Console.ReadKey(true).Key;
                                        if (key == ConsoleKey.UpArrow) selectedOption = (selectedOption - 1 + choices.Length) % choices.Length;
                                        else if (key == ConsoleKey.DownArrow) selectedOption = (selectedOption + 1) % choices.Length;
                                        else if (key == ConsoleKey.Enter)
                                        {
                                            answerSelected = true;

                                            // Check the user's selected answer
                                            if (choices[selectedOption].Equals(correctAnswer, StringComparison.OrdinalIgnoreCase))
                                            {
                                                easyPoints++;
                                                Console.ForegroundColor = ConsoleColor.Green;
                                                Console.WriteLine("\n\t\t\t\t\t\t\t\t\t\tCorrect!.");
                                            }
                                            else
                                            {
                                                Console.ForegroundColor = ConsoleColor.Black;
                                                Console.WriteLine($"\n\t\t\t\t\t\t\t\t\t\tIncorrect.");
                                            }



                                            Console.ResetColor();
                                            Thread.Sleep(1000);
                                        }
                                    }

                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("No data found for the selected IDEQGUESS.");
                                return;

                            }
                        }
                    }

                    Clear();
                    counter++;
                    c++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }


            }

            string username = "";


            //----- adding to HISTORY LEADERBOARD
            // string query = "INSERT INTO historylb (ID, H_EASYPOINTS) VALUES (@id, @heasypoints)";

            string read = "SELECT USER FROM tbinsert WHERE ID = (SELECT MAX(ID) FROM tbinsert)";
            using (MySqlConnection myConn = new MySqlConnection(mycon))
            {
                MySqlCommand myCommand = new MySqlCommand(read, myConn);

                myConn.Open();

                using (MySqlDataReader reader = myCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {

                        username = reader["USER"].ToString();


                    }
                }
            }

            string insert = "INSERT INTO guesslb (ID, NAME, G_EASYPOINTS, G_OVERALLPOINTS) VALUES (@id, @name, @heasyPoints, @G_OVERALLPOINTS);";

            using (MySqlConnection myConn = new MySqlConnection(mycon))
            {
                MySqlCommand myCommand = new MySqlCommand(insert, myConn);
                myCommand.Parameters.AddWithValue("@id", id);
                myCommand.Parameters.AddWithValue("@name", username);
                myCommand.Parameters.AddWithValue("@heasyPoints", holderPoints);
                myCommand.Parameters.AddWithValue("@G_OVERALLPOINTS", holderPoints);
                myConn.Open();
                int rowsAffected = myCommand.ExecuteNonQuery();

            }

            if (holderPoints >= 6)
            {
                string soundFilePath2 = @"C:\congrats.wav";
                ThreadPool.QueueUserWorkItem(myGame.ForEnteringTheNameSounds, soundFilePath2);

             

                int t = 0;

            t:
                if (t >= 1)
                {
                    Clear();
                    myTitles.congrats();
                    Console.Write("\n\t\t\t\t\t\t\t\t\t\t\t\tINVALID SELECTION");
                    Console.Beep(500, 300);
                    Console.WriteLine("\n\n\t\t\t\t\t\t\t\t\t\t   CONGRATULATIONS ON PASSING THIS DIFFICULTIES");
                    Console.WriteLine("\n\t\t\t\t\t\t\t\t\t\t\t\t   YOUR SCORE " + holderPoints); ;
                }
                else
                {
                    Clear();
                    myTitles.congrats();
                    Console.WriteLine("\n\n\t\t\t\t\t\t\t\t\t\t   CONGRATULATIONS ON PASSING THIS DIFFICULTIES");
                    Console.WriteLine("\n\t\t\t\t\t\t\t\t\t\t\t\t   YOUR SCORE " + holderPoints);

                }


  
                Console.Write("\n\t\t\t\t\t\t\t\t\t\t   PRESS X - GO TO MAIN SCREEN\n\t\t\t\t\t\t\t\t\t\t   PRESS M - NEXT DIFFICULTIES (MEDIUM) \n\t\t\t\t\t\t\t\t\t\t   :");
                string decidee = Console.ReadLine().ToLower();


                if (decidee.Equals("m"))
                {



                    isSelected = "GuessMedium";



                    string updateQuery = "UPDATE tbinsert SET DIFFICULTIES = @newDiff WHERE ID = (SELECT MAX(ID) FROM (SELECT ID FROM tbinsert) AS temp);";

                    using (MySqlConnection myConn = new MySqlConnection(mycon))
                    {
                        MySqlCommand updateCommand = new MySqlCommand(updateQuery, myConn);

                        myConn.Open();
                        updateCommand.Parameters.AddWithValue("@newDiff", "Medium");
                        updateCommand.ExecuteNonQuery();
                    }

                    holderPoints = 0;


                    string updateQuery2 = "UPDATE tbinsert SET POINTS = @newPoints WHERE ID = (SELECT MAX(ID) FROM (SELECT ID FROM tbinsert) AS temp);";

                    using (MySqlConnection myConn = new MySqlConnection(mycon))
                    {
                        MySqlCommand updateCommand = new MySqlCommand(updateQuery2, myConn);

                        myConn.Open();
                        updateCommand.Parameters.AddWithValue("@newPoints", holderPoints);
                        updateCommand.ExecuteNonQuery();
                    }

                    mediumGuessQuestionaire();



                }
                else if (decidee.Equals("x"))
                {
                    Console.Clear();
                    myGame.fullscreen();
                    myGame.RunFirstMenu();
                }
                else
                {

                    t++;
                    goto t;

                }


            }
            else
            {
                string soundFilePath2 = @"C:\gameover.wav";
                ThreadPool.QueueUserWorkItem(myGame.ForEnteringTheNameSounds, soundFilePath2);

                int tt = 0;

            tt:
                if (tt >= 1)
                {
                    Clear();
                    myTitles.gameOver();
                    Console.Write("\n\t\t\t\t\t\t\t\t\t\t\t\tINVALID SELECTION");
                    Console.Beep(500, 300);
                    Console.WriteLine("\n\n\t\t\t\t\t\t\t\t\t\t  FAILED, YOU NEED TO MEET THE REQUIRED POINTS (6)");
                    Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t     THE GAME ENDS QUESTION 10/10\n");
                    Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t   YOUR SCORE " + holderPoints);

                }
                else
                {
                    Clear();
                    myTitles.gameOver();
                    Console.WriteLine("\n\n\t\t\t\t\t\t\t\t\t\t  FAILED, YOU NEED TO MEET THE REQUIRED POINTS (6)");
                    Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t     THE GAME ENDS QUESTION 10/10\n");
                    Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t   YOUR SCORE " + holderPoints);

                }



                Console.Write("\n\t\t\t\t\t\t\t\t\t\t  PRESS G - RETRY\n\t\t\t\t\t\t\t\t\t\t  PRESS X - GO TO MAIN SCREEN \n\t\t\t\t\t\t\t\t\t\t  :");
                string decidee = Console.ReadLine().ToLower();

                if (decidee.Equals("g"))
                {


                    easyPoints = 0;
                    easyCounter = 0;
                    Eattempt = 1;
                    holderPoints = 0;



                    string updateQuery = "UPDATE tbinsert SET POINTS = @newPoints WHERE ID = (SELECT MAX(ID) FROM (SELECT ID FROM tbinsert) AS temp);";

                    using (MySqlConnection myConn = new MySqlConnection(mycon))
                    {
                        MySqlCommand updateCommand = new MySqlCommand(updateQuery, myConn);

                        myConn.Open();
                        updateCommand.Parameters.AddWithValue("@newPoints", holderPoints);
                        updateCommand.ExecuteNonQuery();
                    }




                    goto tryEasyAgain;
                }
                else if (decidee.Equals("x"))
                {
                    Console.Clear();
                    myGame.fullscreen();
                    myGame.RunFirstMenu();
                }
                else
                {
                    tt++;

                    goto tt;

                }

            }








        }

        public void mediumGuessQuestionaire()
        {


        tryMediumAgain:
            int counter = 0;
            int c = 0;

            string mycon = "datasource=localhost;Database=algoquest;username=root;convert zero datetime=true";


            while (counter < 7)  
            {

                try
                {
                    HashSet<int> usedIds = new HashSet<int>(); 
                    string randomIdQuery = "SELECT IDMQGUESS FROM tbmediumquestion_guess ORDER BY RAND() LIMIT 1;";
                    int randomIDQ = 0;
                    bool isUniqueIDFound = false;


                    using (MySqlConnection myConn = new MySqlConnection(mycon))
                    {
                        MySqlCommand randomIdCommand = new MySqlCommand(randomIdQuery, myConn);

                        myConn.Open();
                        object result = randomIdCommand.ExecuteScalar();
                        if (result != null)
                        {
                            randomIDQ = Convert.ToInt32(result);
                            if (!usedIds.Contains(randomIDQ))
                            {
                                isUniqueIDFound = true;
                                usedIds.Add(randomIDQ);  
                            }
                        }
                        else
                        {
                            Console.WriteLine("No questions available in the database.");
                            return;
                        }
                    }



                   
                    string query = "SELECT IDMQGUESS, QUESTION, A, B, C, D, ANSWER FROM tbmediumquestion_guess WHERE IDMQGUESS = @IDMQGUESS";

                    using (MySqlConnection myConn = new MySqlConnection(mycon))
                    {
                        MySqlCommand myCommand = new MySqlCommand(query, myConn);
                        myCommand.Parameters.AddWithValue("@IDMQGUESS", randomIDQ);

                        myConn.Open();

                        using (MySqlDataReader reader = myCommand.ExecuteReader())
                        {
                            if (reader.Read())  
                            {
                                string question = reader["QUESTION"].ToString().Replace("  ", "\n\t\t\t\t\t\t\t\t\t");
                                string[] choices = {
                            reader["A"].ToString(),
                            reader["B"].ToString(),
                            reader["C"].ToString(),
                            reader["D"].ToString()
                        };
                                string correctAnswer = reader["ANSWER"].ToString();

                                int selectedOption = 0;
                                bool answerSelected = false;


                                while (!answerSelected)
                                {

                                    Console.Clear();
                                    myTitles.actualGameTitle();
                                    actualGameGuess();
                                    if (c < 6)
                                    {
                                        ResetColor();
                                        Console.BackgroundColor = ConsoleColor.DarkRed;


                                        Console.WriteLine("\n\t\t\t\t\t\t\t\tInstructions: Examine the code, solve the expression step by step following the order of operations, \n\t\t\t\t\t\t\t\t\t and select the,  option that matches your computed result.");
                                        Console.WriteLine($"\n\n\t\t\t\t\t\t\t\t\tQUESTION {counter + 1} - QUESTION 6\n");                                 
                                        Console.WriteLine($"\n\n\t\t\t\t\t\t\t\t\t{question + " "}\n");
                                        Console.WriteLine($"\t\t\t\t\t\t\t\t\t{correctAnswer}");
                                        Console.WriteLine();
                                        Console.WriteLine();
                                        Console.ForegroundColor = ConsoleColor.White;

                                        for (int j = 0; j < choices.Length; j++)
                                        {
                                            if (j == selectedOption)
                                            {
                                                Console.BackgroundColor = ConsoleColor.DarkRed;
                                                Console.ForegroundColor = ConsoleColor.Yellow;
                                            }
                                            else
                                            {
                                                Console.ResetColor();
                                            }
                                            Console.BackgroundColor = ConsoleColor.DarkRed;

                                            Console.WriteLine($"\t\t\t\t\t\t\t\t\t\t\t\t{(char)('A' + j)}. {choices[j]}");
                                        }

                                        // Key input for navigation and selection

                                        var key = Console.ReadKey(true).Key;


                                        if (key == ConsoleKey.UpArrow) selectedOption = (selectedOption - 1 + choices.Length) % choices.Length;
                                        else if (key == ConsoleKey.DownArrow) selectedOption = (selectedOption + 1) % choices.Length;
                                        else if (key == ConsoleKey.Enter)
                                        {
                                            answerSelected = true;

                                            // Check the user's selected answer
                                            char selectedChoice = (char)('A' + selectedOption); // Convert selected index to A, B, C, D



                                            if (selectedChoice.ToString().Equals(correctAnswer, StringComparison.OrdinalIgnoreCase))
                                            {


                                                mediumPoints += 2;
                                                Console.ForegroundColor = ConsoleColor.DarkCyan;
                                                Console.WriteLine("\n\t\t\t\t\t\t\t\t\t\tCorrect!.");



                                            }
                                            else
                                            {
                                                Console.ForegroundColor = ConsoleColor.Black;
                                                Console.WriteLine($"\n\t\t\t\t\t\t\t\t\t\tIncorrect.");


                                            }



                                            Console.ResetColor();
                                            Thread.Sleep(1000);


                                        }
                                    }

                                    else
                                    {
                                        break;
                                    }
                                }

                            }
                            else
                            {
                                Console.WriteLine("No data found for the selected IDMQGUESS.");
                                return;
                            }
                        }
                    }

                    Clear();
                    
                    counter++;
                    c++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }

            Console.Clear();
            ResetColor();
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.DarkRed;



            if (holderPoints >= 8)
            {
                //----- adding to HISTORY LEADERBOARD

                // string query = "INSERT INTO historylb (H_MEDIUMPOINTS) VALUES (@measypoints)";
                // string query = "UPDATE historylb SET H_MEDIUMPOINTS = @measypoints WHERE ID = (SELECT MAX(ID) FROM (SELECT ID FROM historylb) AS temp);";

                string soundFilePath2 = @"C:\congrats.wav";
                ThreadPool.QueueUserWorkItem(myGame.ForEnteringTheNameSounds, soundFilePath2);

                string update = @"
                UPDATE guesslb 
                SET 
                    G_MEDIUMPOINTS = G_MEDIUMPOINTS + @holderPoints,
                    G_OVERALLPOINTS = G_OVERALLPOINTS + @holderPoints
                WHERE ID = @id;";

                using (MySqlConnection myConn = new MySqlConnection(mycon))
                {
                    MySqlCommand myCommand = new MySqlCommand(update, myConn);
                    myCommand.Parameters.AddWithValue("@id", id);             // ID of the row to update
                    myCommand.Parameters.AddWithValue("@holderPoints", holderPoints); // Value to add to both H_MEDIUMPOINTS and H_OVERALLPOINTS
                    myConn.Open();
                    int rowsAffected = myCommand.ExecuteNonQuery();
                }




                int t = 0;

            t:
                if (t >= 1)
                {
                    Clear();
                    myTitles.congrats();
                    Console.Write("\n\t\t\t\t\t\t\t\t\t\t\t\tINVALID SELECTION");
                    Console.Beep(500, 300);
                    Console.WriteLine("\n\n\t\t\t\t\t\t\t\t\t\t   CONGRATULATIONS ON PASSING THIS DIFFICULTIES");
                    Console.WriteLine("\n\t\t\t\t\t\t\t\t\t\t\t\t   YOUR SCORE " + holderPoints); ;
                }
                else
                {
                    Clear();
                    myTitles.congrats();
                    Console.WriteLine("\n\n\t\t\t\t\t\t\t\t\t\t   CONGRATULATIONS ON PASSING THIS DIFFICULTIES");
                    Console.WriteLine("\n\t\t\t\t\t\t\t\t\t\t\t\t   YOUR SCORE " + holderPoints);

                }



                Console.Write("\n\t\t\t\t\t\t\t\t\t\t   PRESS X - GO TO MAIN SCREEN\n\t\t\t\t\t\t\t\t\t\t   PRESS H - NEXT DIFFICULTIES (HARD) \n\t\t\t\t\t\t\t\t\t\t   :");
                string decidee = Console.ReadLine().ToLower();

                if (decidee.Equals("h"))
                {



                    isSelected = "GuessHard";




                    string updateQuery = "UPDATE tbinsert SET DIFFICULTIES = @newDiff WHERE ID = (SELECT MAX(ID) FROM (SELECT ID FROM tbinsert) AS temp);";

                    using (MySqlConnection myConn = new MySqlConnection(mycon))
                    {
                        MySqlCommand updateCommand = new MySqlCommand(updateQuery, myConn);

                        myConn.Open();
                        updateCommand.Parameters.AddWithValue("@newDiff", "Hard");
                        updateCommand.ExecuteNonQuery();
                    }

                    holderPoints = 0;
                    string updateQuery2 = "UPDATE tbinsert SET POINTS = @newPoints WHERE ID = (SELECT MAX(ID) FROM (SELECT ID FROM tbinsert) AS temp);";

                    using (MySqlConnection myConn = new MySqlConnection(mycon))
                    {
                        MySqlCommand updateCommand = new MySqlCommand(updateQuery2, myConn);

                        myConn.Open();
                        updateCommand.Parameters.AddWithValue("@newPoints", holderPoints);
                        updateCommand.ExecuteNonQuery();
                    }

                    hardGuessQuestionaire();


                }
                else if (decidee.Equals("x"))
                {
                    Console.Clear();
                    myGame.fullscreen();
                    myGame.RunFirstMenu();
                }
                else
                {
                    t++;
                    goto t;

                }

            }
            else
            {
                string soundFilePath2 = @"C:\gameover.wav";
                ThreadPool.QueueUserWorkItem(myGame.ForEnteringTheNameSounds, soundFilePath2);







                int tt = 0;

            tt:
                if (tt >= 1)
                {
                    Clear();
                    myTitles.gameOver();
                    Console.Write("\n\t\t\t\t\t\t\t\t\t\t\t\tINVALID SELECTION");
                    Console.Beep(500, 300);
                    Console.WriteLine("\n\n\t\t\t\t\t\t\t\t\t\t  FAILED, YOU NEED TO MEET THE REQUIRED POINTS (8)");
                    Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t     THE GAME ENDS QUESTION 6/6\n");
                    Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t   YOUR SCORE " + holderPoints);
                }
                else
                {
                    Clear();
                    myTitles.gameOver();
                    Console.WriteLine("\n\n\t\t\t\t\t\t\t\t\t\t  FAILED, YOU NEED TO MEET THE REQUIRED POINTS (8)");
                    Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t     THE GAME ENDS QUESTION 6/6\n");
                    Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t   YOUR SCORE " + holderPoints);


                }


                Console.Write("\n\t\t\t\t\t\t\t\t\t\t  PRESS G - RETRY\n\t\t\t\t\t\t\t\t\t\t  PRESS X - GO TO MAIN SCREEN \n\t\t\t\t\t\t\t\t\t\t  :");
                string decidee = Console.ReadLine().ToLower();


                if (decidee.Equals("g"))
                {

                    mediumPoints = 0;
                    mediumCounter = 0;
                    Mattempt = 1;
                    holderPoints = 0;

                    string updateQuery = "UPDATE tbinsert SET POINTS = @newPoints WHERE ID = (SELECT MAX(ID) FROM (SELECT ID FROM tbinsert) AS temp);";

                    using (MySqlConnection myConn = new MySqlConnection(mycon))
                    {
                        MySqlCommand updateCommand = new MySqlCommand(updateQuery, myConn);

                        myConn.Open();
                        updateCommand.Parameters.AddWithValue("@newPoints", holderPoints);
                        updateCommand.ExecuteNonQuery();
                    }

                    goto tryMediumAgain;
                }
                else if (decidee.Equals("x"))
                {
                    Console.Clear();
                    myGame.fullscreen();
                    myGame.RunFirstMenu();
                }
                else
                {
                    tt++;
                    goto tt;

                }

            }

        }




        public void hardGuessQuestionaire()
        {
        tryHardAgain:
            int counter = 0;
            int c = 0;
           
            while (counter < 7)  
            {

                try
                {
                 
                    HashSet<int> usedIds = new HashSet<int>(); 
                    string randomIdQuery = "SELECT IDHQGUESS FROM tbhardquestion_guess ORDER BY RAND() LIMIT 1;";
                    int randomIDQ = 0;
                    bool isUniqueIDFound = false;


                    using (MySqlConnection myConn = new MySqlConnection(mycon))
                    {
                        MySqlCommand randomIdCommand = new MySqlCommand(randomIdQuery, myConn);

                        myConn.Open();
                        object result = randomIdCommand.ExecuteScalar();
                        if (result != null)
                        {
                            randomIDQ = Convert.ToInt32(result);
                            if (!usedIds.Contains(randomIDQ))
                            {
                                isUniqueIDFound = true;
                                usedIds.Add(randomIDQ);  
                            }
                        }
                        else
                        {
                            Console.WriteLine("No questions available in the database.");
                            return;
                        }
                    }



                    string query = "SELECT IDHQGUESS, QUESTION, ANSWER FROM  tbhardquestion_guess WHERE IDHQGUESS = @IDHQGUESS";

                    using (MySqlConnection myConn = new MySqlConnection(mycon))
                    {
                        MySqlCommand myCommand = new MySqlCommand(query, myConn);
                        myCommand.Parameters.AddWithValue("@IDHQGUESS", randomIDQ);

                        myConn.Open();

                        using (MySqlDataReader reader = myCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string question = reader["QUESTION"].ToString().Replace("  ", "\n\t\t\t\t\t\t\t\t\t");
                                string correctAnswer = reader["ANSWER"].ToString().ToLower();

                                Console.Clear();
                                myTitles.actualGameTitle();
                                actualGameGuess();
                                if (c < 6)
                                {
                                    ResetColor();
                                    Console.BackgroundColor = ConsoleColor.DarkRed;
                                    Console.WriteLine("\n\t\t\t\t\t\t\t\tInstructions:Be a Human Compiler! Carefully read the code, pay close attention to spacing in the Console.Write   \n\t\t\t\t\t\t\t\t     output, compute the division for each iteration of the loop step by step, and ensure accurate \n\t\t\t\t\t\t\t\t\t\t    placement of spaces between the numbers in the result.  ");

                                    Console.WriteLine($"\n\n\t\t\t\t\t\t\t\t\tQUESTION {counter + 1} - QUESTION 6\n");
                                    Console.WriteLine($"\t\t\t\t\t\t\t\t\t{question + " "}\n");

                                    Console.ForegroundColor = ConsoleColor.White;
                                    Write("\t\t\t\t\t\t\t\t\t: " + correctAnswer);
                                    Write("\n\t\t\t\t\t\t\t\t\tAnswer: ");
                                    string ans = Console.ReadLine().ToLower();

                                    if (ans.Equals(correctAnswer, StringComparison.OrdinalIgnoreCase))
                                    {
                                        hardPoints += 3;
                                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                                        Console.WriteLine("\n\t\t\t\t\t\t\t\t\t\tCorrect!");
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.Black;
                                        Console.WriteLine($"\n\t\t\t\t\t\t\t\t\t\tIncorrect!");
                                        Console.ResetColor();
                                    }

                                    Thread.Sleep(1000);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                Console.WriteLine("No data found for the selected IDHQGUESS.");
                                return;
                            }
                            
                          

                        }
                    }

                    Clear();
                    counter++;
                    c++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }

            }

            Console.Clear();
            ResetColor();
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.DarkRed;


            if (holderPoints >= 12)
            {
                string soundFilePath2 = @"C:\passed.wav";
                ThreadPool.QueueUserWorkItem(myGame.ForEnteringTheNameSounds, soundFilePath2);
                //----- adding to HISTORY LEADERBOARD

                string update = @"
                UPDATE guesslb 
                SET 
                    G_HARDPOINTS = G_HARDPOINTS + @holderPoints,
                    G_OVERALLPOINTS = G_OVERALLPOINTS + @holderPoints
                WHERE ID = @id;";

                using (MySqlConnection myConn = new MySqlConnection(mycon))
                {
                    MySqlCommand myCommand = new MySqlCommand(update, myConn);
                    myCommand.Parameters.AddWithValue("@id", id);             // ID of the row to update
                    myCommand.Parameters.AddWithValue("@holderPoints", holderPoints); // Value to add to both H_MEDIUMPOINTS and H_OVERALLPOINTS
                    myConn.Open();
                    int rowsAffected = myCommand.ExecuteNonQuery();
                }



                string updateQuery = "UPDATE tbinsert SET DIFFICULTIES = @newDiff, POINTS = @newPoints, GUESSCOUNTER = @GUESSCOUNTER, DONE = @newSelc WHERE ID = (SELECT MAX(ID) FROM (SELECT ID FROM tbinsert) AS temp);";

              

                using (MySqlConnection myConn = new MySqlConnection(mycon))
                {
                    MySqlCommand updateCommand = new MySqlCommand(updateQuery, myConn);


                    updateCommand.Parameters.AddWithValue("@newDiff", "Easy");
                    updateCommand.Parameters.AddWithValue("@newPoints", 0);
                    updateCommand.Parameters.AddWithValue("@GUESSCOUNTER", 1);
                    updateCommand.Parameters.AddWithValue("@newSelc", "SELECT_G");
         
                    myConn.Open();
                    updateCommand.ExecuteNonQuery();
                }



                int t = 0;


            t:
                if (t >= 1)
                {
                    Clear();
                    myTitles.passed();
                    Console.Write("\n\t\t\t\t\t\t\t\t\t\t\t\tINVALID SELECTION");
                    Console.Beep(500, 300);
                    Console.WriteLine("\n\n\t\t\t\t\t\t\t\t\t\t   CONGRATULATIONS ON PASSING THIS MODE/SUBJECT");
                    Console.WriteLine("\n\t\t\t\t\t\t\t\t\t\t\t\t   YOUR SCORE " + holderPoints); ;
                }
                else
                {
                    Clear();
                    myTitles.passed();
                    Console.WriteLine("\n\n\t\t\t\t\t\t\t\t\t\t   CONGRATULATIONS ON PASSING THIS MODE/SUBJECT");
                    Console.WriteLine("\n\t\t\t\t\t\t\t\t\t\t\t\t   YOUR SCORE " + holderPoints);

                }



                Console.Write("\n\t\t\t\t\t\t\t\t\t\t   PRESS 0 - TO CHOOSE MODES AGAIN\n\t\t\t\t\t\t\t\t\t\t   PRESS X - GO TO MAIN SCREEN \n\t\t\t\t\t\t\t\t\t\t   :");
                string decidee = Console.ReadLine().ToLower();

                if (decidee.Equals("0"))
                {



                    //////////----------------------------------------------------------------------------------------------------------------------------------------------

                    holderPoints = 0;

                    easyPoints = 0;
                    easyCounter = 0;
                    Eattempt = 1;

                    mediumPoints = 0;
                    mediumCounter = 0;
                    Mattempt = 1;

                    hardPoints = 0;
                    hardCounter = 0;
                    Hattempt = 1;

                  

                     
                    pms.ModesSelectionCondition();






                }
                else if (decidee.Equals("x"))
                {
                    Console.Clear();
                    myGame.fullscreen();
                    myGame.RunFirstMenu();
                }
                else
                {
                    t++;
                    goto t;

                }

            }
            else
            {

                string soundFilePath2 = @"C:\gameover.wav";
                ThreadPool.QueueUserWorkItem(myGame.ForEnteringTheNameSounds, soundFilePath2);



                int tt = 0;

            tt:
                if (tt >= 1)
                {
                    Clear();
                    myTitles.gameOver();
                    Console.Write("\n\t\t\t\t\t\t\t\t\t\t  Invalid Selection..");
                    Console.Beep(500, 300);
                    Console.WriteLine("\n\t\t\t\t\t\t\t\t\t\t  YOUR SCORE " + holderPoints);
                    Console.WriteLine("\n\t\t\t\t\t\t\t\t\t\t  FAILED. YOU NEED TO MEET THE REQUIRED POINTS (12). ");
                    Console.WriteLine("\t\t\t\t\t\t\t\t\t\t  THE GAME ENDS QUESTION 6/6\n");
                }
                else
                {
                    Clear();
                    myTitles.gameOver();
                    Console.WriteLine("\n\n\t\t\t\t\t\t\t\t\t\t  YOUR SCORE " + holderPoints);
                    Console.WriteLine("\n\t\t\t\t\t\t\t\t\t\t  FAILED. YOU NEED TO MEET THE REQUIRED POINTS (12). ");
                    Console.WriteLine("\t\t\t\t\t\t\t\t\t\t  THE GAME ENDS QUESTION 6/6\n");








                    Console.Write("\t\t\t\t\t\t\t\t\t\t  PRESS X - GO TO MAIN SCREEN\n\t\t\t\t\t\t\t\t\t\t  PRESS G - RETRY\n\t\t\t\t\t\t\t\t\t\t  :");
                    string decidee = Console.ReadLine().ToLower();

                    if (decidee.Equals("g"))
                    {

                        hardPoints = 0;
                        hardCounter = 0;
                        Hattempt = 1;
                        holderPoints = 0;

                        string updateQuery = "UPDATE tbinsert SET POINTS = @newPoints WHERE ID = (SELECT MAX(ID) FROM (SELECT ID FROM tbinsert) AS temp);";

                        using (MySqlConnection myConn = new MySqlConnection(mycon))
                        {
                            MySqlCommand updateCommand = new MySqlCommand(updateQuery, myConn);

                            myConn.Open();
                            updateCommand.Parameters.AddWithValue("@newPoints", holderPoints);
                            updateCommand.ExecuteNonQuery();
                        }

                        goto tryHardAgain;
                    }
                    else if (decidee.Equals("x"))
                    {
                        Console.Clear();
                        myGame.fullscreen();
                        myGame.RunFirstMenu();
                    }
                    else
                    {
                        tt++;
                        goto tt;


                    }

                }

            }

        }







        public void actualGameGuess()
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Clear();

            myTitles.actualGameTitle();

            int points = 0;



            try
            {
                string query = "SELECT ID, USER, MODES, DIFFICULTIES, POINTS FROM tbinsert WHERE ID = (SELECT MAX(ID) FROM tbinsert)";
                string mycon2 = "datasource=localhost;Database=algoquest;username=root;convert zero datetime=true";



                using (MySqlConnection myConn = new MySqlConnection(mycon2))
                {
                    MySqlCommand myCommand = new MySqlCommand(query, myConn);
                    myConn.Open();

                    using (MySqlDataReader reader = myCommand.ExecuteReader())
                    {
                        if (reader.Read()) // Check if a row is returned
                        {
                            // Retrieve values from the row
                            id = Convert.ToInt32(reader["ID"]);
                            string username = reader["USER"].ToString();
                            string modes = reader["MODES"].ToString();
                            string difficulties = reader["DIFFICULTIES"].ToString();
                            points = Convert.ToInt32(reader["POINTS"]);

                            Console.ForegroundColor = ConsoleColor.Yellow;
                            WriteLine("\n\n\t\t\t| USERNAME: " + username);
                            WriteLine("\t\t\t| USER ID: " + id);
                            WriteLine("\n\n\t\t\t| MODE: " + modes);
                            WriteLine("\t\t\t| DIFFICULTY: " + difficulties);


                            // Close the reader here before executing other commands
                            reader.Close(); // Close the reader before executing the update query
                        }
                        else
                        {
                            Console.WriteLine("No data found in the table.");
                            return;
                        }
                    }

                    // Additional logic for easy/medium/hard points


                    if (isSelected.Equals("GuessEasy"))
                    {
                        if (Eattempt >= 2)
                        {
                            easyPoints -= 1;
                            Eattempt--;

                        }

                        if (easyPoints == 0)
                        {
                            holderPoints = points; // Set holderPoints to points if easyPoints is 0
                            WriteLine("\n\n\t\t\t| POINTS: " + holderPoints);
                        }
                        else
                        {
                            holderPoints = points + easyPoints;
                            WriteLine("\n\n\t\t\t| POINTS: " + holderPoints);
                            Eattempt++;
                        }
                    }
                    else if (isSelected.Equals("GuessMedium"))
                    {

                        if (Mattempt >= 2)
                        {
                            mediumPoints -= 2;
                            Mattempt--;
                            //}else if (attempt >= 3) {

                            //    mediumPoints -= mediumPoints;

                        }

                        if (mediumPoints == 0)
                        {
                            holderPoints = points;
                            WriteLine("\n\n\t\t\t| POINTS: " + holderPoints);


                        }
                        else

                        {


                            holderPoints = points + mediumPoints;

                            WriteLine("\n\n\t\t\t| POINTS: " + holderPoints);
                            Mattempt++;
                        }


                    }
                    else if (isSelected.Equals("GuessHard"))
                    {
                        if (Hattempt >= 2)
                        {
                            hardPoints -= 3;
                            Hattempt--;
                            //}else if (attempt >= 3) {

                            //    mediumPoints -= mediumPoints;

                        }

                        if (hardPoints == 0)
                        {
                            holderPoints = points;
                            WriteLine("\n\n\t\t\t| POINTS: " + holderPoints);


                        }
                        else

                        {


                            holderPoints = points + hardPoints;

                            WriteLine("\n\n\t\t\t| POINTS: " + holderPoints);
                            Hattempt++;
                        }
                    }

                    // Now update the points in the database
                    string updateQuery = "UPDATE tbinsert SET POINTS = @newPoints WHERE ID = (SELECT MAX(ID) FROM (SELECT ID FROM tbinsert) AS temp);";
                    using (MySqlCommand updateCommand = new MySqlCommand(updateQuery, myConn))
                    {
                        // Use the calculated holderPoints for updating
                        updateCommand.Parameters.AddWithValue("@newPoints", holderPoints);
                        updateCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }


    }


}