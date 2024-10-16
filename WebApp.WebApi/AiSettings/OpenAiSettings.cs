using WebApp.WebApi.Interfaces;

namespace WebApp.WebApi.AiSettings;

public class OpenAiSettings : IOpenAiSettings
{
    public string GetPostPrompt(string title)
    {
        var role = "Content-maker";
        var topics =
            "TopicId 1 = Sport" +
            "TopicId 2 = Technology" +
            "TopicId 3 = Self-Development" +
            "TopicId 4 = Health & Wellness" +
            "TopicId 5 = Finance" +
            "TopicId 6 = Education" +
            "TopicId 7 = Travel" +
            "TopicId 8 = Productivity" +
            "TopicId 9 = Books & Literature" +
            "TopicId 10 = Entertainment";

        return $@"You are {role}. Your goal is to write content for the post based on its title: '{title}'. 
                Select the most appropriate topic from the following list (by TopicId) and generate tags for it:
                {topics}.
                Provide the result in the form of a JSON object. An example of JSON object:
                {{
                    ""Content"": ""Sample content based on the title"",
                    ""TopicId"": 2,
                    ""Tags"": [""tag1"", ""tag2""]
                }}";
    }
}
