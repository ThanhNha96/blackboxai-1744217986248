cusing UnityEngine;
using System.Collections.Generic;

public class RacingTournament : MonoBehaviour
{
    public string tournamentName = "Giải Bát Hương Vàng Mở Rộng";
    public List<Transform> raceTracks; // List of race track transforms
    public List<VehicleController> participants; // List of vehicles participating in the race
    public int maxParticipants = 8; // Maximum number of participants
    public float raceDuration = 300f; // Duration of the race in seconds
    public PoliceChaseSystem policeChaseSystem; // Reference to the police chase system

    private bool isRaceActive = false;

    void Start()
    {
        // Initialize the tournament
        InitializeTournament();
    }

    void InitializeTournament()
    {
        // Set up race tracks and participants
        if (raceTracks.Count == 0)
        {
            Debug.LogError("No race tracks assigned for the tournament.");
            return;
        }

        // Randomly select participants
        for (int i = 0; i < maxParticipants; i++)
        {
            // Assuming VehicleController is a script attached to the vehicle prefabs
            VehicleController vehicle = Instantiate(Resources.Load<VehicleController>("Prefabs/VehiclePrefab"));
            participants.Add(vehicle);
        }

        Debug.Log($"{tournamentName} has been initialized with {participants.Count} participants.");
    }

    public void StartRace()
    {
        if (isRaceActive)
        {
            Debug.LogWarning("Race is already in progress.");
            return;
        }

        isRaceActive = true;
        Debug.Log($"The race '{tournamentName}' has started!");

        // Start the race logic
        StartCoroutine(RaceCoroutine());
    }

    private IEnumerator RaceCoroutine()
    {
        float timeElapsed = 0f;

        while (timeElapsed < raceDuration)
        {
            timeElapsed += Time.deltaTime;
            // Check for police chase
            policeChaseSystem.CheckForChase(participants);
            yield return null;
        }

        EndRace();
    }

    private void EndRace()
    {
        isRaceActive = false;
        Debug.Log($"The race '{tournamentName}' has ended!");

        // Determine the winner and display results
        VehicleController winner = DetermineWinner();
        Debug.Log($"The winner is: {winner.name}");
    }

    private VehicleController DetermineWinner()
    {
        // Logic to determine the winner based on positions or times
        // For simplicity, return the first participant as the winner
        return participants[0];
    }
}
