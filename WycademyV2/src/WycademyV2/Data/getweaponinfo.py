import requests
import re
import json

weapons = ["greatsword", "longsword", "swordshield", "dualblades", "hammer", "huntinghorn", "switchaxe", "chargeblade",
           "insectglaive", "lance", "gunlance", "lightbowgun", "heavybowgun", "bow"]

for weapon in weapons:
    response = requests.get("http://mhgen.kiranico.com/%s" % weapon)

    start_index = response.text.index(r'[{"id":')
    end_index = re.search(r'"upgrades_from_level":.+}]', response.text).end()

    substring = response.text[start_index:end_index]

    with open(r"./weapon/%s.json" % weapon, "w+", encoding="utf-8") as f:
        json_from_string = json.loads(substring)
        json.dump(json_from_string, f, indent=4)
