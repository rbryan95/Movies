// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Routing;
using NLog;


class Program
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    static void Main(string[] args)
    {
        try
        {
            MovieDataManager dataManager = new MovieDataManager();
            

            while (true)
            {
                Console.WriteLine("Movie Console Application");
                Console.WriteLine("1. List Movies");
                Console.WriteLine("2. Add Movie");
                Console.WriteLine("3. Quit");
                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();
                
                switch (choice)
                {
                    case "1":
                    List<Movie> movies = dataManager.LoadMovies();
                        
                        foreach (Movie movie in movies)
                        {
                            Console.WriteLine($"{movie.ID}, {movie.Title}, {movie.Category}");
                        }
                        continue;
                    case "2":
                        Console.Write("Enter movie ID: ");
                        if(!int.TryParse(Console.ReadLine(), out int ID))
                        {
                            Console.WriteLine("Invalid ID.");
                            break;
                        }
                        Console.Write("Enter movie title: ");                        
                        string Title = Console.ReadLine();

                        Console.Write("Enter Category: ");
                        string Category = Console.ReadLine();

                        if (dataManager.AddMovie("Movies.csv", ID.ToString(), Title, Category))
                        {
                            Console.WriteLine("Movie added successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Movie not added due to duplicate ID or Title.");
                        }
                        break;
                    case "3":
                        dataManager.SaveMovies(dataManager.LoadMovies());
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            logger.Error(ex, "An unexpected error occurred.");
            Console.WriteLine("An unexpected error occurred. Please check the log for details.");
        }
    }
}


public class Movie
{
    public string ID { get; set; }
    public string Title { get; set; }
    public int Category { get; set; }
}



public class MovieDataManager
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

  // private const string Movies.csv = "movies.csv";

    public List<Movie> LoadMovies()
    {
        List<Movie> movies = new List<Movie>();

        try
        {
            if (File.Exists("Movies.csv"))
            {
                string[] lines = File.ReadAllLines("Movies.csv");
                foreach (string line in lines)
                {
                    string[] parts = line.Split(',');
                    if (parts.Length == 3 && int.TryParse(parts[0], out int ID))
                    {
                        movies.Add(new Movie {ID = ID.ToString(), Title = parts[1], Category = int.Parse(parts[2])});
                    }
                    else
                    {
                        logger.Error("Invalid line in the CSV file" + line);
                    }
                }
                return movies;
            }
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error loading movies from file");
        }

        return movies;
    }

    public void SaveMovies(List<Movie> movies)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter("Movies.csv"))
            {
                foreach (Movie movie in movies)
                {
                    writer.WriteLine($"{movie.ID},{movie.Title},{movie.Category}");
                }
            }
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error saving movies to file");
        }
    }

    public bool AddMovie(string filepath, string movieId, string movieTitle, string movieCategory)
        //List<Movie> movies, Movie movie)
        
    {
        string newMovie = $"{movieId}, {movieTitle}, {movieCategory}";
        if (File.Exists(filepath))
        //Any(m => m.ID.Equals(movie.ID, StringComparison.OrdinalIgnoreCase)))
        {
            File.WriteAllLines(filepath, new[] {newMovie});
            return true;
        }
        var lines = File.ReadAllLines(filepath);

        foreach (var line in lines)
        {
            var fields = line.Split(",");
            if (fields[0].Trim()== movieId || fields[1].Trim() == movieTitle)
            {
                return false;
            }
        }
        lines = lines.Concat(new[] { newMovie }).ToArray();

        File.AppendAllText(filepath, newMovie + Environment.NewLine);
        return true;
      
    }
}
