/**
 * ImageImporter.js
 * Utility to dynamically import all images from the assets folder
 */

// Import all images
function importAll(r) {
  const images = {};
  Object.keys(r).forEach((item) => {
    // Extract the filename without extension as the key
    const key = item.replace("./", "").replace(/\.(jpeg|jpg|png|svg)$/, "");
    images[key] = item;
  });
  return images;
}

// Import all images from the assets directory
const images = importAll(
  import.meta.glob("@/assets/*.{jpeg,jpg,png,svg}", { eager: true })
);


export default images;
