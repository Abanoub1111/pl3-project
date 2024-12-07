open System
open System.Windows.Forms
open System.Drawing
open Newtonsoft.Json
open System.IO

// File paths for saving and loading data
let dictionaryFilePath = "dictionary.json"

// Pure function to add a word to the dictionary
let addWord (word: string) (definition: string) (dictionary: Map<string, string>) =
    if dictionary.ContainsKey(word.ToLower()) then
        Error "Word already exists!"
    else
        let updatedDictionary = dictionary.Add(word.ToLower(), definition)
        Ok updatedDictionary

// Pure function to update a word's definition in the dictionary
let updateWord (word: string) (definition: string) (dictionary: Map<string, string>) =
    if dictionary.ContainsKey(word.ToLower()) then
        let updatedDictionary = dictionary.Add(word.ToLower(), definition)
        Ok updatedDictionary
    else
        Error "Word not found!"

// Pure function to delete a word from the dictionary
let deleteWord (word: string) (dictionary: Map<string, string>) =
    if dictionary.ContainsKey(word.ToLower()) then
        let updatedDictionary = dictionary.Remove(word.ToLower())
        Ok updatedDictionary
    else
        Error "Word not found!"

// Pure function to search for words starting with a keyword
let searchWords (keyword: string) (dictionary: Map<string, string>) =
    dictionary
    |> Map.filter (fun key _ -> key.StartsWith(keyword.ToLower()))
    |> Map.toSeq
    |> Seq.map (fun (k, v) -> $"{k}: {v}")
    |> Seq.toArray

// Function to save the dictionary to a JSON file (side-effect)
let saveDictionaryToFile dictionary =
    try
        let json = JsonConvert.SerializeObject(dictionary)
        File.WriteAllText(dictionaryFilePath, json)
    with ex -> 
        MessageBox.Show($"Failed to save dictionary: {ex.Message}") |> ignore

// Function to load the dictionary from a JSON file (side-effect)
let loadDictionaryFromFile () =
    try
        if File.Exists(dictionaryFilePath) then
            let json = File.ReadAllText(dictionaryFilePath)
            JsonConvert.DeserializeObject<Map<string, string>>(json)
        else
            Map.empty
    with ex -> 
        MessageBox.Show($"Failed to load dictionary: {ex.Message}") |> ignore
        Map.empty

// Initial state
let mutable dictionary = loadDictionaryFromFile()

// Create the form
let form = new Form(Text = "Digital Dictionary", Width = 700, Height = 460, BackColor = Color.FromArgb(18, 24, 36))
form.Font <- new Font("Segoe UI Semibold", 11.0f)
form.MaximizeBox <- false
form.MinimumSize <- form.Size
form.MaximumSize <- form.Size

// Controls
let lblWord = new Label(Text = "Enter Word", Location = Point(297, 40), ForeColor = Color.FromArgb(255, 190, 72), Width = 200)
lblWord.Font <- new Font("Segoe UI Semibold", 14.0f)
let txtWord = new TextBox(Location = Point(275, 66), Width = 150, ForeColor = Color.White, BackColor = Color.FromArgb(23, 30, 46), BorderStyle = BorderStyle.None)

let lblDefinition = new Label(Text = "Enter Definition", Location = Point(279, 100), ForeColor = Color.FromArgb(255, 190, 72), Width = 200)
lblDefinition.Font <- new Font("Segoe UI Semibold", 14.0f)
let txtDefinition = new TextBox(Location = Point(150, 126), Width = 400, Height = 60, ForeColor = Color.White, Multiline = true, BackColor = Color.FromArgb(23, 30, 46), BorderStyle = BorderStyle.None)

let btnAdd = new Button(Text = "Add", Location = Point(180, 206), Width = 100, ForeColor = Color.White, BackColor = Color.FromArgb(124, 26, 26), FlatStyle = FlatStyle.Flat)
btnAdd.FlatAppearance.BorderSize <- 0
let btnUpdate = new Button(Text = "Update", Location = Point(300, 206), Width = 100, ForeColor = Color.White, BackColor = Color.FromArgb(124, 26, 26), FlatStyle = FlatStyle.Flat)
btnUpdate.FlatAppearance.BorderSize <- 0
let btnDelete = new Button(Text = "Delete", Location = Point(420, 206), Width = 100, ForeColor = Color.White, BackColor = Color.FromArgb(124, 26, 26), FlatStyle = FlatStyle.Flat)
btnDelete.FlatAppearance.BorderSize <- 0

let lblSearch = new Label(Text = "Search Here", Location = Point(200, 247), ForeColor = Color.FromArgb(255, 190, 72), Width = 200)
lblSearch.Font <- new Font("Segoe UI Semibold", 14.0f)
let txtSearch = new TextBox(Location = Point(180, 280), Width = 150, ForeColor = Color.White, BackColor = Color.FromArgb(23, 30, 46), BorderStyle = BorderStyle.None)

let lstSuggestions = new ListBox(Location = Point(180, 297), Width = 150, Height = 75, ForeColor = Color.White, BackColor = Color.FromArgb(23, 30, 46), BorderStyle = BorderStyle.None)
lstSuggestions.Visible <- false

let btnSearch = new Button(Text = "Search", Location = Point(420, 280), Width = 100, ForeColor = Color.White, BackColor = Color.FromArgb(124, 26, 26), FlatStyle = FlatStyle.Flat)
btnSearch.FlatAppearance.BorderSize <- 0

let lstResults = new ListBox(Location = Point(100, 330), Width = 500, Height = 80, ForeColor = Color.White, BackColor = Color.FromArgb(23, 30, 46), BorderStyle = BorderStyle.None)

// Event Handlers
btnAdd.Click.Add(fun _ -> 
    match addWord txtWord.Text txtDefinition.Text dictionary with
    | Ok updatedDictionary ->
        dictionary <- updatedDictionary
        saveDictionaryToFile dictionary
        MessageBox.Show("Word added successfully!") |> ignore
        txtWord.Clear()
        txtDefinition.Clear()
    | Error msg -> 
        MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning) |> ignore)

btnUpdate.Click.Add(fun _ -> 
    match updateWord txtWord.Text txtDefinition.Text dictionary with
    | Ok updatedDictionary ->
        dictionary <- updatedDictionary
        saveDictionaryToFile dictionary
        MessageBox.Show("Word updated successfully!") |> ignore
        txtWord.Clear()
        txtDefinition.Clear()
    | Error msg -> 
        MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning) |> ignore)

btnDelete.Click.Add(fun _ -> 
    match deleteWord txtWord.Text dictionary with
    | Ok updatedDictionary ->
        dictionary <- updatedDictionary
        saveDictionaryToFile dictionary
        MessageBox.Show("Word deleted successfully!") |> ignore
        txtWord.Clear()
    | Error msg -> 
        MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning) |> ignore)

txtSearch.TextChanged.Add(fun _ -> 
    let suggestions = 
        searchWords txtSearch.Text dictionary
        |> Array.map (fun item -> item.Split(':').[0]) // Extract only the word for suggestions
    lstSuggestions.Items.Clear()
    lstSuggestions.Items.AddRange(suggestions |> Array.map box)
    lstSuggestions.Visible <- suggestions.Length > 0)

lstSuggestions.SelectedIndexChanged.Add(fun _ -> 
    if lstSuggestions.SelectedIndex >= 0 then
        txtSearch.Text <- lstSuggestions.SelectedItem.ToString()
        lstSuggestions.Items.Clear()
        lstSuggestions.Visible <- false)

btnSearch.Click.Add(fun _ -> 
    let results = searchWords txtSearch.Text dictionary
    lstResults.Items.Clear()
    if results.Length = 0 then
        MessageBox.Show("No matching word found!", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
    else
        lstResults.Items.AddRange(results |> Array.map box))

form.Controls.AddRange([| lblWord; txtWord; lblDefinition; txtDefinition
                          btnAdd; btnUpdate; btnDelete
                          lblSearch; txtSearch; btnSearch; lstSuggestions; lstResults |])

[<STAThread>]
Application.Run(form)
