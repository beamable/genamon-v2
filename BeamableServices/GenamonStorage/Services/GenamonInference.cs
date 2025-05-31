using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.Microservices.Pokemon.Storage;
using Beamable.Microservices.Pokemon.Storage.Models;
using Beamable.Server;
using Newtonsoft.Json;
using UnityEngine;

public class GenamonInference
{
    private readonly OpenAI _openAI;
    private readonly IStorageObjectConnectionProvider _storage;
    private const string Prompt = "Generate a json object describing a made-up pokemon based on the original 150 pokemon and the original gameboy game. The character sheet should include name, type (array of human-readable), stats (numeric), abilities (human-readable), moves (human-readable), and description (briefly describe the appearance). The stats should include hp, atk, def, sp.atk, sp.def, and speed. All json key names should be lowercase, and the json object should be minified.";
    private const string ImagePrompt = "Generate an image of a made-up pokemon consistent with the style of the original 150 pokemon. Ensure there is no text, and a stylized abstract background based on the description. Use the following description of the pokemon:\n\n";

    public GenamonInference(OpenAI openAI, IStorageObjectConnectionProvider storage)
    {
        _openAI = openAI;
        _storage = storage;
    }

    public async Task Generate(int quantity)
    {
        var db = await _storage.GenamonStorageDatabase();
        var completionResponse = await _openAI.SendChatCompletion(Prompt, quantity);
        var pokemons = new List<PokemonText>();
        foreach (var choice in completionResponse.choices)
        {
            if (choice.finish_reason == "stop")
            {
                PokemonText pokemon = null;
                try
                {
                    pokemon = JsonConvert.DeserializeObject<PokemonText>(choice.message.content.Trim());
                }
                catch(Exception e)
                {
                    Debug.LogException(e);
                }
                finally
                {
                    if (pokemon != null)
                    {
                        pokemons.Add(pokemon);
                    }
                }
            }
        }
        
        if(pokemons.Count == 0)
        {
            return;
        }

        var records = new List<PendingPokemon>();
        var tasks = pokemons.Select(pokemon => _openAI.SendImageGeneration($"{ImagePrompt}{pokemon.description}"));
        var results = await Task.WhenAll(tasks);

        for (int i = 0; i < pokemons.Count; i++)
        {
            records.Add(new PendingPokemon
            {
                Pokemon = pokemons[i],
                ImageUrl = results[i].data.First().url
            });
        }
        
        var insertSuccessful = await PendingPokemonCollection.TryInsert(db, records);
        if (!insertSuccessful)
        {
            throw new MicroserviceException(500, "FailedToInsert", "Inserting pending pokemon failed!");
        }
    }
}
